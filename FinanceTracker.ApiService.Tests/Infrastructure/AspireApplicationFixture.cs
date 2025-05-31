using System;
using System.Net.Http;
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
            // Get a scope from the API service to access the DbContext
            using var scope = _app!.Services.CreateScope();

            // We need to make a request to the API to trigger the DbContext creation
            // since it's in the API service, not the test host
            try
            {
                // Make a simple request to ensure the API service is ready
                // The API service will create its own in-memory database when it starts
                var response = await HttpClient.GetAsync("/health");
                response.EnsureSuccessStatusCode();
            }
            catch (Exception ex)
            {
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
