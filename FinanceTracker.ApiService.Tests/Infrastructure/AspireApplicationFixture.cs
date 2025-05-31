using System;
using System.Net.Http;
using System.Threading.Tasks;
using Aspire.Hosting;
using Aspire.Hosting.Testing;
using Microsoft.Extensions.DependencyInjection;
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
            // Create the Aspire application host
            var appHost = await DistributedApplicationTestingBuilder.CreateAsync<Projects.FinanceTracker_AppHost>();

            // Configure HTTP client defaults
            appHost.Services.ConfigureHttpClientDefaults(clientBuilder =>
            {
                clientBuilder.AddStandardResilienceHandler();
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
