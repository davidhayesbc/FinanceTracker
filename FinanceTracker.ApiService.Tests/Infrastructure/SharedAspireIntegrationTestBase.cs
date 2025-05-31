using System.Net.Http;
using System.Threading.Tasks;
using FinanceTracker.ApiService.Tests.Infrastructure;
using Xunit;

namespace FinanceTracker.ApiService.Tests.Infrastructure
{
    /// <summary>
    /// Base class for integration tests using the shared Aspire application fixture
    /// This provides access to the HttpClient while ensuring the application is shared across tests
    /// </summary>
    [Collection("AspireApplication")]
    public abstract class SharedAspireIntegrationTestBase : IAsyncLifetime
    {
        protected readonly AspireApplicationFixture _fixture;
        protected HttpClient HttpClient => _fixture.HttpClient;

        protected SharedAspireIntegrationTestBase(AspireApplicationFixture fixture)
        {
            _fixture = fixture;
        }

        /// <summary>
        /// Called before each test method
        /// Override to provide test-specific setup
        /// </summary>
        public virtual Task InitializeAsync()
        {
            return Task.CompletedTask;
        }

        /// <summary>
        /// Called after each test method
        /// Override to provide test-specific cleanup
        /// </summary>
        public virtual Task DisposeAsync()
        {
            return Task.CompletedTask;
        }
    }
}
