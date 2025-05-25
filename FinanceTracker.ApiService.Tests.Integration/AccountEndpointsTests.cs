using Microsoft.Extensions.DependencyInjection;
using FinanceTracker.Data.Models;
using FinanceTracker.ApiService.Dtos;
using System.Net.Http.Json;
using System.Net;

namespace FinanceTracker.ApiService.Tests.Integration;

public class AccountEndpointsTests : IClassFixture<TestWebApplicationFactory>
{
    private readonly TestWebApplicationFactory _factory;
    private readonly HttpClient _client;

    public AccountEndpointsTests(TestWebApplicationFactory factory)
    {
        _factory = factory;
        _client = _factory.CreateClient();
    }

    [Fact]
    public async Task CreateCashAccount_WithValidData_ReturnsCreatedAccount()
    {
        // Arrange
        var request = new CreateCashAccountRequestDto
        {
            Name = "Test Checking Account",
            Institution = "Test Bank",
            AccountTypeId = 1,
            CurrencyId = 1,
            IsActive = true,
            InitialBalance = 1000.00m
        };

        // Act
        var response = await _client.PostAsJsonAsync("/accounts/cash", request);

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var createdAccount = await response.Content.ReadFromJsonAsync<CashAccountDto>();
        Assert.NotNull(createdAccount);
        Assert.Equal(request.Name, createdAccount.Name);
        Assert.Equal(request.Institution, createdAccount.Institution);
        Assert.Equal(request.IsActive, createdAccount.IsActive);
        Assert.Equal(request.InitialBalance, createdAccount.CurrentBalance);
        Assert.True(createdAccount.Id > 0);
    }

    [Fact]
    public async Task CreateCashAccount_WithInvalidAccountType_ReturnsBadRequest()
    {
        // Arrange
        var request = new CreateCashAccountRequestDto
        {
            Name = "Test Account",
            Institution = "Test Bank",
            AccountTypeId = 999, // Non-existent account type
            CurrencyId = 1,
            IsActive = true,
            InitialBalance = 1000.00m
        };

        // Act
        var response = await _client.PostAsJsonAsync("/accounts/cash", request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task CreateCashAccount_WithInvalidCurrency_ReturnsBadRequest()
    {
        // Arrange
        var request = new CreateCashAccountRequestDto
        {
            Name = "Test Account",
            Institution = "Test Bank",
            AccountTypeId = 1,
            CurrencyId = 999, // Non-existent currency
            IsActive = true,
            InitialBalance = 1000.00m
        };

        // Act
        var response = await _client.PostAsJsonAsync("/accounts/cash", request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task CreateCashAccount_WithEmptyName_ReturnsBadRequest()
    {
        // Arrange
        var request = new CreateCashAccountRequestDto
        {
            Name = "", // Empty name should fail validation
            Institution = "Test Bank",
            AccountTypeId = 1,
            CurrencyId = 1,
            IsActive = true,
            InitialBalance = 1000.00m
        };

        // Act
        var response = await _client.PostAsJsonAsync("/accounts/cash", request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
}
