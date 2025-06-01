using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using FluentAssertions;
using FinanceTracker.ApiService.Dtos;
using FinanceTracker.ApiService.Tests.Infrastructure;

namespace FinanceTracker.ApiService.Tests;

/// <summary>
/// Integration tests for Security endpoints (/securities)
/// Tests all security-related API endpoints
/// </summary>
[Collection("AspireApplication")]
public class SecurityEndpointsIntegrationTests : SharedAspireIntegrationTestBase
{
    public SecurityEndpointsIntegrationTests(AspireApplicationFixture fixture) : base(fixture)
    {
    }

    private async Task<CurrencyDto> CreateTestCurrency(string name = "US Dollar", string symbol = "USD", string displaySymbol = "$")
    {
        var createRequest = new CreateCurrencyRequestDto
        {
            Name = name,
            Symbol = symbol,
            DisplaySymbol = displaySymbol
        };

        var response = await HttpClient.PostAsJsonAsync("/api/v1/currencies", createRequest);
        response.StatusCode.Should().Be(HttpStatusCode.Created);

        return await response.Content.ReadFromJsonAsync<CurrencyDto>() ?? throw new InvalidOperationException("Failed to create currency");
    }

    #region Get All Securities Tests

    [Fact]
    public async Task GetAllSecurities_WithNoSecurities_ReturnsEmptyList()
    {
        // Act
        var response = await HttpClient.GetAsync("/api/v1/securities");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var securities = await response.Content.ReadFromJsonAsync<List<SecurityDto>>();
        securities.Should().NotBeNull();
        securities.Should().BeEmpty();
    }

    [Fact]
    public async Task GetAllSecurities_WithSecurities_ReturnsAllSecurities()
    {
        // Arrange
        var currency = await CreateTestCurrency();

        var createRequest = new CreateSecurityRequestDto
        {
            Symbol = "AAPL",
            Name = "Apple Inc.",
            ISIN = "US0378331005",
            CurrencyId = currency.Id,
            SecurityType = "Stock"
        };

        // Create a security first
        var createResponse = await HttpClient.PostAsJsonAsync("/api/v1/securities", createRequest);
        createResponse.StatusCode.Should().Be(HttpStatusCode.Created);

        // Act
        var response = await HttpClient.GetAsync("/api/v1/securities");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var securities = await response.Content.ReadFromJsonAsync<List<SecurityDto>>();
        securities.Should().NotBeNull();
        securities.Should().HaveCount(1);
        securities![0].Symbol.Should().Be("AAPL");
        securities[0].Name.Should().Be("Apple Inc.");
        securities[0].CurrencySymbol.Should().Be("USD");
    }

    #endregion

    #region Get Security by ID Tests

    [Fact]
    public async Task GetSecurityById_WithValidId_ReturnsSecurity()
    {
        // Arrange
        var currency = await CreateTestCurrency();

        var createRequest = new CreateSecurityRequestDto
        {
            Symbol = "MSFT",
            Name = "Microsoft Corporation",
            ISIN = "US5949181045",
            CurrencyId = currency.Id,
            SecurityType = "Stock"
        };

        var createResponse = await HttpClient.PostAsJsonAsync("/api/v1/securities", createRequest);
        var createdSecurity = await createResponse.Content.ReadFromJsonAsync<SecurityDto>();

        // Act
        var response = await HttpClient.GetAsync($"/api/v1/securities/{createdSecurity!.Id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var security = await response.Content.ReadFromJsonAsync<SecurityDto>();
        security.Should().NotBeNull();
        security!.Id.Should().Be(createdSecurity.Id);
        security.Symbol.Should().Be("MSFT");
        security.Name.Should().Be("Microsoft Corporation");
        security.CurrencySymbol.Should().Be("USD");
    }

    [Fact]
    public async Task GetSecurityById_WithInvalidId_ReturnsNotFound()
    {
        // Act
        var response = await HttpClient.GetAsync("/api/v1/securities/999");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    #endregion

    #region Get Security by Symbol Tests

    [Fact]
    public async Task GetSecurityBySymbol_WithValidSymbol_ReturnsSecurity()
    {
        // Arrange
        var currency = await CreateTestCurrency();

        var createRequest = new CreateSecurityRequestDto
        {
            Symbol = "GOOGL",
            Name = "Alphabet Inc.",
            ISIN = "US02079K3059",
            CurrencyId = currency.Id,
            SecurityType = "Stock"
        };

        var createResponse = await HttpClient.PostAsJsonAsync("/api/v1/securities", createRequest);
        createResponse.StatusCode.Should().Be(HttpStatusCode.Created);

        // Act
        var response = await HttpClient.GetAsync("/api/v1/securities/by-symbol/GOOGL");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var security = await response.Content.ReadFromJsonAsync<SecurityDto>();
        security.Should().NotBeNull();
        security!.Symbol.Should().Be("GOOGL");
        security.Name.Should().Be("Alphabet Inc.");
    }

    [Fact]
    public async Task GetSecurityBySymbol_WithInvalidSymbol_ReturnsNotFound()
    {
        // Act
        var response = await HttpClient.GetAsync("/api/v1/securities/by-symbol/INVALID");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    #endregion

    #region Create Security Tests

    [Fact]
    public async Task CreateSecurity_WithValidData_ReturnsCreatedSecurity()
    {
        // Arrange
        var currency = await CreateTestCurrency();

        var createRequest = new CreateSecurityRequestDto
        {
            Symbol = "TSLA",
            Name = "Tesla, Inc.",
            ISIN = "US88160R1014",
            CurrencyId = currency.Id,
            SecurityType = "Stock"
        };

        // Act
        var response = await HttpClient.PostAsJsonAsync("/api/v1/securities", createRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var security = await response.Content.ReadFromJsonAsync<SecurityDto>();
        security.Should().NotBeNull();
        security!.Id.Should().BeGreaterThan(0);
        security.Symbol.Should().Be("TSLA");
        security.Name.Should().Be("Tesla, Inc.");
        security.ISIN.Should().Be("US88160R1014");
        security.CurrencyId.Should().Be(currency.Id);
        security.SecurityType.Should().Be("Stock");

        // Verify Location header
        response.Headers.Location.Should().NotBeNull();
        response.Headers.Location!.ToString().Should().Contain($"/api/v1/securities/{security.Id}");
    }

    [Fact]
    public async Task CreateSecurity_WithDuplicateSymbol_ReturnsBadRequest()
    {
        // Arrange
        var currency = await CreateTestCurrency();

        var createRequest1 = new CreateSecurityRequestDto
        {
            Symbol = "AMZN",
            Name = "Amazon.com, Inc.",
            CurrencyId = currency.Id,
            SecurityType = "Stock"
        };

        var createRequest2 = new CreateSecurityRequestDto
        {
            Symbol = "AMZN", // Same symbol
            Name = "Amazon Inc.",
            CurrencyId = currency.Id,
            SecurityType = "Stock"
        };

        // Create first security
        await HttpClient.PostAsJsonAsync("/api/v1/securities", createRequest1);

        // Act - Try to create second security with same symbol
        var response = await HttpClient.PostAsJsonAsync("/api/v1/securities", createRequest2);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CreateSecurity_WithInvalidCurrency_ReturnsBadRequest()
    {
        // Arrange
        var createRequest = new CreateSecurityRequestDto
        {
            Symbol = "NVDA",
            Name = "NVIDIA Corporation",
            CurrencyId = 999, // Invalid currency ID
            SecurityType = "Stock"
        };

        // Act
        var response = await HttpClient.PostAsJsonAsync("/api/v1/securities", createRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CreateSecurity_WithInvalidData_ReturnsBadRequest()
    {
        // Arrange
        var currency = await CreateTestCurrency();

        var createRequest = new CreateSecurityRequestDto
        {
            Symbol = "", // Invalid - required
            Name = "Some Security",
            CurrencyId = currency.Id,
            SecurityType = "Stock"
        };

        // Act
        var response = await HttpClient.PostAsJsonAsync("/api/v1/securities", createRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    #endregion

    #region Update Security Tests

    [Fact]
    public async Task UpdateSecurity_WithValidData_ReturnsUpdatedSecurity()
    {
        // Arrange
        var currency = await CreateTestCurrency();

        var createRequest = new CreateSecurityRequestDto
        {
            Symbol = "META",
            Name = "Meta Platforms",
            CurrencyId = currency.Id,
            SecurityType = "Stock"
        };

        var createResponse = await HttpClient.PostAsJsonAsync("/api/v1/securities", createRequest);
        var createdSecurity = await createResponse.Content.ReadFromJsonAsync<SecurityDto>();

        var updateRequest = new UpdateSecurityRequestDto
        {
            Symbol = "META",
            Name = "Meta Platforms, Inc.", // Updated name
            CurrencyId = currency.Id,
            SecurityType = "Stock"
        };

        // Act
        var response = await HttpClient.PutAsJsonAsync($"/api/v1/securities/{createdSecurity!.Id}", updateRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var updatedSecurity = await response.Content.ReadFromJsonAsync<SecurityDto>();
        updatedSecurity.Should().NotBeNull();
        updatedSecurity!.Id.Should().Be(createdSecurity.Id);
        updatedSecurity.Name.Should().Be("Meta Platforms, Inc.");
        updatedSecurity.Symbol.Should().Be("META");
    }

    [Fact]
    public async Task UpdateSecurity_WithInvalidId_ReturnsNotFound()
    {
        // Arrange
        var currency = await CreateTestCurrency();

        var updateRequest = new UpdateSecurityRequestDto
        {
            Symbol = "XYZ",
            Name = "Some Security",
            CurrencyId = currency.Id,
            SecurityType = "Stock"
        };

        // Act
        var response = await HttpClient.PutAsJsonAsync("/api/v1/securities/999", updateRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    #endregion

    #region Delete Security Tests

    [Fact]
    public async Task DeleteSecurity_WithValidId_ReturnsNoContent()
    {
        // Arrange
        var currency = await CreateTestCurrency();

        var createRequest = new CreateSecurityRequestDto
        {
            Symbol = "TEST",
            Name = "Test Security",
            CurrencyId = currency.Id,
            SecurityType = "Stock"
        };

        var createResponse = await HttpClient.PostAsJsonAsync("/api/v1/securities", createRequest);
        var createdSecurity = await createResponse.Content.ReadFromJsonAsync<SecurityDto>();

        // Act
        var response = await HttpClient.DeleteAsync($"/api/v1/securities/{createdSecurity!.Id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        // Verify security is deleted
        var getResponse = await HttpClient.GetAsync($"/api/v1/securities/{createdSecurity.Id}");
        getResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task DeleteSecurity_WithInvalidId_ReturnsNotFound()
    {
        // Act
        var response = await HttpClient.DeleteAsync("/api/v1/securities/999");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    #endregion

    #region Get Security Types Tests

    [Fact]
    public async Task GetSecurityTypes_WithNoSecurities_ReturnsEmptyList()
    {
        // Act
        var response = await HttpClient.GetAsync("/api/v1/securities/types");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var securityTypes = await response.Content.ReadFromJsonAsync<List<string>>();
        securityTypes.Should().NotBeNull();
        securityTypes.Should().BeEmpty();
    }

    [Fact]
    public async Task GetSecurityTypes_WithSecurities_ReturnsDistinctTypes()
    {
        // Arrange
        var currency = await CreateTestCurrency();

        var stockRequest = new CreateSecurityRequestDto
        {
            Symbol = "STOCK1",
            Name = "Stock Security",
            CurrencyId = currency.Id,
            SecurityType = "Stock"
        };

        var bondRequest = new CreateSecurityRequestDto
        {
            Symbol = "BOND1",
            Name = "Bond Security",
            CurrencyId = currency.Id,
            SecurityType = "Bond"
        };

        var anotherStockRequest = new CreateSecurityRequestDto
        {
            Symbol = "STOCK2",
            Name = "Another Stock",
            CurrencyId = currency.Id,
            SecurityType = "Stock" // Duplicate type
        };

        // Create securities
        await HttpClient.PostAsJsonAsync("/api/v1/securities", stockRequest);
        await HttpClient.PostAsJsonAsync("/api/v1/securities", bondRequest);
        await HttpClient.PostAsJsonAsync("/api/v1/securities", anotherStockRequest);

        // Act
        var response = await HttpClient.GetAsync("/api/v1/securities/types");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var securityTypes = await response.Content.ReadFromJsonAsync<List<string>>();
        securityTypes.Should().NotBeNull();
        securityTypes.Should().HaveCount(2); // Should have distinct types only
        securityTypes.Should().Contain("Bond");
        securityTypes.Should().Contain("Stock");
    }

    #endregion
}
