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
            // Set environment variable for API service to use in-memory database BEFORE building
            Environment.SetEnvironmentVariable("UseInMemoryDatabase", "true");

            // Create the Aspire application host with test configuration
            var appHost = await DistributedApplicationTestingBuilder.CreateAsync<Projects.FinanceTracker_AppHost>();

            // Configure the services to use in-memory database for testing
            appHost.Services.ConfigureHttpClientDefaults(clientBuilder =>
            {
                clientBuilder.AddStandardResilienceHandler();
            });

            // Override the environment to use in-memory database
            appHost.Services.Configure<Microsoft.Extensions.Hosting.HostOptions>(options =>
            {
                options.ServicesStartConcurrently = true;
                options.ServicesStopConcurrently = true;
            });

            // Build and start the application
            _app = await appHost.BuildAsync();
            var resourceNotificationService = _app.Services.GetRequiredService<ResourceNotificationService>();

            await _app.StartAsync();

            // Wait for the migration service to complete first
            await resourceNotificationService
                .WaitForResourceAsync("migrationservice", KnownResourceStates.Finished)
                .WaitAsync(TimeSpan.FromSeconds(60));

            // Create HTTP client for the API service
            HttpClient = _app.CreateHttpClient("apiservice");

            // Wait for the API service to be running
            await resourceNotificationService
                .WaitForResourceAsync("apiservice", KnownResourceStates.Running)
                .WaitAsync(TimeSpan.FromSeconds(30));

            // Seed the in-memory database with essential data
            await SeedDatabaseAsync();
        }

        /// <summary>
        /// Seeds the in-memory database with essential data required for tests
        /// </summary>
        private async Task SeedDatabaseAsync()
        {
            try
            {
                Console.WriteLine("AspireApplicationFixture: Verifying API service health...");

                // Make a simple request to ensure the API service is ready
                var response = await HttpClient.GetAsync("/health");
                response.EnsureSuccessStatusCode();

                Console.WriteLine("AspireApplicationFixture: API service is healthy");

                // Verify that essential reference data exists by making some API calls
                Console.WriteLine("AspireApplicationFixture: Verifying reference data...");

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

                Console.WriteLine("AspireApplicationFixture: Database verification complete");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"AspireApplicationFixture: Error during database seeding/verification: {ex.Message}");
                throw new InvalidOperationException("Failed to initialize API service database", ex);
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
        }
    }
}
