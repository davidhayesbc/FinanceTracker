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
    /// Base class for all integration tests using .NET Aspire
    /// Provides common setup and teardown functionality
    /// </summary>
    public abstract class AspireIntegrationTestBase : IAsyncLifetime
    {
        protected HttpClient HttpClient { get; private set; } = null!;
        private DistributedApplication? _app;

        /// <summary>
        /// Initialize the Aspire application and HTTP client
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

            // Perform any additional setup
            await OnInitializeAsync();
        }

        /// <summary>
        /// Clean up resources
        /// </summary>
        public async Task DisposeAsync()
        {
            await OnDisposeAsync();

            HttpClient?.Dispose();
            if (_app != null)
            {
                await _app.DisposeAsync();
            }
        }

        /// <summary>
        /// Override this method to provide custom initialization logic
        /// </summary>
        protected virtual Task OnInitializeAsync() => Task.CompletedTask;

        /// <summary>
        /// Override this method to provide custom cleanup logic
        /// </summary>
        protected virtual Task OnDisposeAsync() => Task.CompletedTask;
    }
}
