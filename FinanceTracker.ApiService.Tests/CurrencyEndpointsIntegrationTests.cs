using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using FluentAssertions;
using FinanceTracker.ApiService.Dtos;
using FinanceTracker.ApiService.Tests.Infrastructure;

namespace FinanceTracker.ApiService.Tests;

/// <summary>
/// Integration tests for Currency endpoints (/currencies)
/// Tests all currency-related API endpoints
/// </summary>
[Collection("AspireApplication")]
public class CurrencyEndpointsIntegrationTests : SharedAspireIntegrationTestBase
{
    public CurrencyEndpointsIntegrationTests(AspireApplicationFixture fixture) : base(fixture)
    {
    }

    #region Get All Currencies Tests

    [Fact]
    public async Task GetAllCurrencies_WithNoCurrencies_ReturnsEmptyList()
    {
        // Arrange
        // HttpClient is already available from base class

        // Act
        var response = await HttpClient.GetAsync("/api/v1/currencies");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var currencies = await response.Content.ReadFromJsonAsync<List<CurrencyDto>>();
        currencies.Should().NotBeNull();
        currencies.Should().BeEmpty();
    }

    [Fact]
    public async Task GetAllCurrencies_WithCurrencies_ReturnsAllCurrencies()
    {
        // Arrange
        var createRequest = new CreateCurrencyRequestDto
        {
            Name = "US Dollar",
            Symbol = "USD",
            DisplaySymbol = "$"
        };

        // Create a currency first
        var createResponse = await HttpClient.PostAsJsonAsync("/api/v1/currencies", createRequest);
        createResponse.StatusCode.Should().Be(HttpStatusCode.Created);

        // Act
        var response = await HttpClient.GetAsync("/api/v1/currencies");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var currencies = await response.Content.ReadFromJsonAsync<List<CurrencyDto>>();
        currencies.Should().NotBeNull();
        currencies.Should().HaveCount(1);
        currencies![0].Name.Should().Be("US Dollar");
        currencies[0].Symbol.Should().Be("USD");
        currencies[0].DisplaySymbol.Should().Be("$");
    }

    #endregion

    #region Get Currency by ID Tests

    [Fact]
    public async Task GetCurrencyById_WithValidId_ReturnsCurrency()
    {
        // Arrange
        var createRequest = new CreateCurrencyRequestDto
        {
            Name = "Euro",
            Symbol = "EUR",
            DisplaySymbol = "€"
        };

        var createResponse = await HttpClient.PostAsJsonAsync("/api/v1/currencies", createRequest);
        var createdCurrency = await createResponse.Content.ReadFromJsonAsync<CurrencyDto>();

        // Act
        var response = await HttpClient.GetAsync($"/api/v1/currencies/{createdCurrency!.Id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var currency = await response.Content.ReadFromJsonAsync<CurrencyDto>();
        currency.Should().NotBeNull();
        currency!.Id.Should().Be(createdCurrency.Id);
        currency.Name.Should().Be("Euro");
        currency.Symbol.Should().Be("EUR");
        currency.DisplaySymbol.Should().Be("€");
    }

    [Fact]
    public async Task GetCurrencyById_WithInvalidId_ReturnsNotFound()
    {
        // Act
        var response = await HttpClient.GetAsync("/api/v1/currencies/999");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    #endregion

    #region Create Currency Tests

    [Fact]
    public async Task CreateCurrency_WithValidData_ReturnsCreatedCurrency()
    {
        // Arrange
        var createRequest = new CreateCurrencyRequestDto
        {
            Name = "British Pound",
            Symbol = "GBP",
            DisplaySymbol = "£"
        };

        // Act
        var response = await HttpClient.PostAsJsonAsync("/api/v1/currencies", createRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var currency = await response.Content.ReadFromJsonAsync<CurrencyDto>();
        currency.Should().NotBeNull();
        currency!.Id.Should().BeGreaterThan(0);
        currency.Name.Should().Be("British Pound");
        currency.Symbol.Should().Be("GBP");
        currency.DisplaySymbol.Should().Be("£");

        // Verify Location header
        response.Headers.Location.Should().NotBeNull();
        response.Headers.Location!.ToString().Should().Contain($"/api/v1/currencies/{currency.Id}");
    }

    [Fact]
    public async Task CreateCurrency_WithDuplicateSymbol_ReturnsBadRequest()
    {
        // Arrange
        var createRequest1 = new CreateCurrencyRequestDto
        {
            Name = "US Dollar",
            Symbol = "USD",
            DisplaySymbol = "$"
        };

        var createRequest2 = new CreateCurrencyRequestDto
        {
            Name = "United States Dollar",
            Symbol = "USD", // Same symbol
            DisplaySymbol = "US$"
        };

        // Create first currency
        await HttpClient.PostAsJsonAsync("/api/v1/currencies", createRequest1);

        // Act - Try to create second currency with same symbol
        var response = await HttpClient.PostAsJsonAsync("/api/v1/currencies", createRequest2);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CreateCurrency_WithInvalidData_ReturnsBadRequest()
    {
        // Arrange
        var createRequest = new CreateCurrencyRequestDto
        {
            Name = "", // Invalid - required
            Symbol = "JPY",
            DisplaySymbol = "¥"
        };

        // Act
        var response = await HttpClient.PostAsJsonAsync("/api/v1/currencies", createRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    #endregion

    #region Update Currency Tests

    [Fact]
    public async Task UpdateCurrency_WithValidData_ReturnsUpdatedCurrency()
    {
        // Arrange
        var createRequest = new CreateCurrencyRequestDto
        {
            Name = "Japanese Yen",
            Symbol = "JPY",
            DisplaySymbol = "¥"
        };

        var createResponse = await HttpClient.PostAsJsonAsync("/api/v1/currencies", createRequest);
        var createdCurrency = await createResponse.Content.ReadFromJsonAsync<CurrencyDto>();

        var updateRequest = new UpdateCurrencyRequestDto
        {
            Name = "Japanese Yen (Updated)",
            Symbol = "JPY",
            DisplaySymbol = "¥"
        };

        // Act
        var response = await HttpClient.PutAsJsonAsync($"/api/v1/currencies/{createdCurrency!.Id}", updateRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var updatedCurrency = await response.Content.ReadFromJsonAsync<CurrencyDto>();
        updatedCurrency.Should().NotBeNull();
        updatedCurrency!.Id.Should().Be(createdCurrency.Id);
        updatedCurrency.Name.Should().Be("Japanese Yen (Updated)");
        updatedCurrency.Symbol.Should().Be("JPY");
    }

    [Fact]
    public async Task UpdateCurrency_WithInvalidId_ReturnsNotFound()
    {
        // Arrange
        var updateRequest = new UpdateCurrencyRequestDto
        {
            Name = "Some Currency",
            Symbol = "SCY",
            DisplaySymbol = "S"
        };

        // Act
        var response = await HttpClient.PutAsJsonAsync("/api/v1/currencies/999", updateRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    #endregion

    #region Delete Currency Tests

    [Fact]
    public async Task DeleteCurrency_WithValidId_ReturnsNoContent()
    {
        // Arrange
        var createRequest = new CreateCurrencyRequestDto
        {
            Name = "Test Currency",
            Symbol = "TEST",
            DisplaySymbol = "T"
        };

        var createResponse = await HttpClient.PostAsJsonAsync("/api/v1/currencies", createRequest);
        var createdCurrency = await createResponse.Content.ReadFromJsonAsync<CurrencyDto>();

        // Act
        var response = await HttpClient.DeleteAsync($"/api/v1/currencies/{createdCurrency!.Id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        // Verify currency is deleted
        var getResponse = await HttpClient.GetAsync($"/api/v1/currencies/{createdCurrency.Id}");
        getResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task DeleteCurrency_WithInvalidId_ReturnsNotFound()
    {
        // Act
        var response = await HttpClient.DeleteAsync("/api/v1/currencies/999");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    #endregion
}
