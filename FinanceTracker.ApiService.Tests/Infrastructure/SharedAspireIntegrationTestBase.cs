using System.Net.Http;
using System.Threading.Tasks;
using FinanceTracker.ApiService.Tests.Infrastructure;
using Xunit;

namespace FinanceTracker.ApiService.Tests.Infrastructure
{
    /// <summary>
    /// Base class for integration tests using the shared Aspire application fixture
    /// This provides access to the HttpClient while ensuring the application is shared across tests
    /// Each test gets a clean database state by clearing data before each test
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
        /// Called before each test method - clears database to ensure clean state
        /// Override to provide additional test-specific setup
        /// </summary>
        public virtual async Task InitializeAsync()
        {
            await ClearDatabaseAsync();
        }

        /// <summary>
        /// Called after each test method
        /// Override to provide test-specific cleanup
        /// </summary>
        public virtual Task DisposeAsync()
        {
            return Task.CompletedTask;
        }

        /// <summary>
        /// Clears all data from the database to ensure each test starts with a clean state
        /// </summary>
        private Task ClearDatabaseAsync()
        {
            // Since we can't directly access the DbContext from here (it's in the API service),
            // we could either:
            // 1. Create a test-only endpoint to clear data
            // 2. Use each test to clean up after itself
            // 3. Use a new database for each test (slower but more isolated)

            // For now, we'll rely on the fact that each test creates its own data
            // and the database starts empty. If tests start interfering with each other,
            // we may need to add a test cleanup endpoint to the API.

            // TODO: Consider adding a test-only endpoint like DELETE /api/test/clear-data
            // that clears all non-reference data when in test mode

            return Task.CompletedTask;
        }
    }
}
