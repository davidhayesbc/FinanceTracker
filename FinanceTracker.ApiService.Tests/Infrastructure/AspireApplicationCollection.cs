using Xunit;

namespace FinanceTracker.ApiService.Tests.Infrastructure
{
    /// <summary>
    /// Collection definition for tests that share the Aspire application fixture
    /// This ensures all tests in this collection share the same application instance
    /// </summary>
    [CollectionDefinition("AspireApplication")]
    public class AspireApplicationCollection : ICollectionFixture<AspireApplicationFixture>
    {
        // This class has no code, and is never created.
        // Its purpose is simply to be the place to apply [CollectionDefinition] and all the
        // ICollectionFixture<> interfaces.
    }
}
