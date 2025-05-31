using FinanceTracker.ApiService.Tests.Infrastructure;

namespace FinanceTracker.ApiService.Tests.Tests
{
    public class IntegrationTest1 : SharedAspireIntegrationTestBase
    {
        public IntegrationTest1(AspireApplicationFixture fixture) : base(fixture)
        {
        }

        [Fact]
        public async Task GetWebResourceRootReturnsOkStatusCode()
        {
            // Arrange
            // HttpClient is already available from base class
            // To output logs to the xUnit.net ITestOutputHelper, consider adding a package from https://www.nuget.org/packages?q=xunit+logging

            // Act
            var response = await HttpClient.GetAsync("/swagger");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }
    }
}
