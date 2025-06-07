using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Aspire.Hosting;
using Aspire.Hosting.Testing;
using Microsoft.Extensions.DependencyInjection;
using FinanceTracker.Data.Models;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace FinanceTracker.ApiService.Tests.Infrastructure
{
    /// <summary>
    /// Shared fixture that starts the Aspire application once and reuses it across all tests
    /// This significantly improves test performance by avoiding repeated application startup/shutdown
    /// </summary>
    public class AspireApplicationFixture : IAsyncLifetime
    {
        private DistributedApplication? _app;

        public HttpClient HttpClient { get; private set; } = null!;

        /// <summary>
        /// Initialize the Aspire application and HTTP client once for all tests
        /// </summary>
        public async Task InitializeAsync()
        {
            // Set environment to Testing to ensure separate database mount
            Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "Testing");

            // Create the Aspire application host with test configuration
            var appHost = await DistributedApplicationTestingBuilder.CreateAsync<Projects.FinanceTracker_AppHost>();

            // Configure the services for testing
            appHost.Services.ConfigureHttpClientDefaults(clientBuilder =>
            {
                clientBuilder.AddStandardResilienceHandler();
            });

            // Configure for faster test execution
            appHost.Services.Configure<Microsoft.Extensions.Hosting.HostOptions>(options =>
            {
                options.ServicesStartConcurrently = true;
                options.ServicesStopConcurrently = true;
            });

            // Build and start the application
            _app = await appHost.BuildAsync();
            var resourceNotificationService = _app.Services.GetRequiredService<ResourceNotificationService>();

            await _app.StartAsync();

            // Wait for the migration service to complete first - this will set up the database
            await resourceNotificationService
                .WaitForResourceAsync("migrationservice", KnownResourceStates.Finished)
                .WaitAsync(TimeSpan.FromSeconds(60));

            // Create HTTP client for the API service
            HttpClient = _app.CreateHttpClient("apiservice");

            // Wait for the API service to be running
            await resourceNotificationService
                .WaitForResourceAsync("apiservice", KnownResourceStates.Running)
                .WaitAsync(TimeSpan.FromSeconds(30));

            // Verify the setup by checking API health
            await VerifyApiServiceAsync();
        }

        /// <summary>
        /// Verifies that the API service is ready and seeded data is available
        /// </summary>
        private async Task VerifyApiServiceAsync()
        {
            try
            {
                Console.WriteLine("AspireApplicationFixture: Verifying API service health...");

                // Make a simple request to ensure the API service is ready
                var response = await HttpClient.GetAsync("/health");
                response.EnsureSuccessStatusCode();

                Console.WriteLine("AspireApplicationFixture: API service is healthy");

                // Verify that essential reference data exists (seeded by migration service)
                Console.WriteLine("AspireApplicationFixture: Verifying seeded reference data...");

                var accountTypesResponse = await HttpClient.GetAsync("/api/v1/accountTypes");
                if (accountTypesResponse.IsSuccessStatusCode)
                {
                    var accountTypes = await accountTypesResponse.Content.ReadFromJsonAsync<List<dynamic>>();
                    Console.WriteLine($"AspireApplicationFixture: Found {accountTypes?.Count ?? 0} account types");
                }
                else
                {
                    Console.WriteLine($"AspireApplicationFixture: Failed to check account types - Status: {accountTypesResponse.StatusCode}");
                }

                var transactionTypesResponse = await HttpClient.GetAsync("/api/v1/transactionTypes");
                if (transactionTypesResponse.IsSuccessStatusCode)
                {
                    var transactionTypes = await transactionTypesResponse.Content.ReadFromJsonAsync<List<dynamic>>();
                    Console.WriteLine($"AspireApplicationFixture: Found {transactionTypes?.Count ?? 0} transaction types");
                }
                else
                {
                    Console.WriteLine($"AspireApplicationFixture: Failed to check transaction types - Status: {transactionTypesResponse.StatusCode}");
                }

                Console.WriteLine("AspireApplicationFixture: API service verification complete");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"AspireApplicationFixture: Error during API service verification: {ex.Message}");
                throw new InvalidOperationException("Failed to verify API service setup", ex);
            }
        }

        /// <summary>
        /// Clean up resources when all tests are complete
        /// </summary>
        public async Task DisposeAsync()
        {
            HttpClient?.Dispose();
            if (_app != null)
            {
                await _app.DisposeAsync();
            }

            // Reset environment variable
            Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", null);
        }
    }
}
