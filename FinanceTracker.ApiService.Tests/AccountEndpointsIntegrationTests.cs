using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using FluentAssertions;
using FinanceTracker.ApiService.Dtos;
using FinanceTracker.ApiService.Tests.Infrastructure;

namespace FinanceTracker.ApiService.Tests;

/// <summary>
/// Integration tests for Account endpoints (/accounts)
/// Tests all account-related API endpoints including cash and investment accounts
/// </summary>
// Use the AspireApplicationFixture for shared app context
[Collection("AspireApplication")]
public class AccountEndpointsIntegrationTests : SharedAspireIntegrationTestBase
{
    public AccountEndpointsIntegrationTests(AspireApplicationFixture fixture) : base(fixture)
    {
    }

    #region Get All Accounts Tests

    [Fact]
    public async Task GetAllAccounts_WithNoAccounts_ReturnsEmptyList()
    {
        // Arrange
        // HttpClient is already available from base class

        // Act
        var response = await HttpClient.GetAsync("/api/v1/accounts");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var accounts = await response.Content.ReadFromJsonAsync<List<AccountBaseDto>>();
        accounts.Should().NotBeNull();
        accounts.Should().BeEmpty();
    }

    #endregion

    #region Get Cash Accounts Tests

    [Fact]
    public async Task GetCashAccounts_WithNoCashAccounts_ReturnsEmptyList()
    {
        // Arrange
        // HttpClient is already available from base class

        // Act
        var response = await HttpClient.GetAsync("/api/v1/accounts/cash");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var accounts = await response.Content.ReadFromJsonAsync<List<CashAccountDto>>();
        accounts.Should().NotBeNull();
        accounts.Should().BeEmpty();
    }

    #endregion

    #region Get Investment Accounts Tests

    [Fact]
    public async Task GetInvestmentAccounts_WithNoInvestmentAccounts_ReturnsEmptyList()
    {
        // Arrange
        // HttpClient is already available from base class

        // Act
        var response = await HttpClient.GetAsync("/api/v1/accounts/investment");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var accounts = await response.Content.ReadFromJsonAsync<List<InvestmentAccountDto>>();
        accounts.Should().NotBeNull();
        accounts.Should().BeEmpty();
    }

    #endregion

    #region Get Account by ID Tests

    [Fact]
    public async Task GetAccountById_WithInvalidId_ReturnsNotFound()
    {
        // Arrange
        var invalidId = 999;

        // Act
        var response = await HttpClient.GetAsync($"/api/v1/accounts/{invalidId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    #endregion

    #region Get Account Transactions Tests

    [Fact]
    public async Task GetAccountTransactions_WithInvalidAccountId_ReturnsNotFound()
    {
        // Arrange
        var invalidId = 999;

        // Act
        var response = await HttpClient.GetAsync($"/api/v1/accounts/{invalidId}/transactions");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    #endregion

    #region Get Account Recurring Transactions Tests

    [Fact]
    public async Task GetAccountRecurringTransactions_WithInvalidAccountId_ReturnsNotFound()
    {
        // Arrange
        var invalidId = 999;

        // Act
        var response = await HttpClient.GetAsync($"/api/v1/accounts/{invalidId}/recurringTransactions");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    #endregion

    #region Create Cash Account Tests

    [Fact]
    public async Task CreateCashAccount_WithValidData_ReturnsCreatedAndCorrectAccount()
    {
        // Arrange
        var request = new CreateCashAccountRequestDto
        {
            Name = "Test Cash Account",
            Institution = "Test Bank",
            AccountTypeId = 1,
            CurrencyId = 1,
            InitialBalance = 123.45m,
            OverdraftLimit = 500.00m,
            IsActive = true
        };

        // Act
        var response = await HttpClient.PostAsJsonAsync("/api/v1/accounts/cash", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var created = await response.Content.ReadFromJsonAsync<CashAccountDto>();
        created.Should().NotBeNull();
        created!.Id.Should().BeGreaterThan(0);
        created.Name.Should().Be(request.Name);
        created.CurrentBalance.Should().Be(request.InitialBalance);
        created.OverdraftLimit.Should().Be(request.OverdraftLimit);
    }

    [Fact]
    public async Task CreateCashAccount_WithMissingRequiredFields_ReturnsBadRequest()
    {
        // Arrange: missing Name, AccountTypeId, CurrencyId
        var request = new CreateCashAccountRequestDto { Institution = "Test Bank" };

        // Act
        var response = await HttpClient.PostAsJsonAsync("/api/v1/accounts/cash", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CreateCashAccount_WithInvalidAccountTypeId_ReturnsBadRequest()
    {
        // Arrange: use seeded currency, invalid account type
        var request = new CreateCashAccountRequestDto
        {
            Name = "Test Cash Account",
            Institution = "Test Bank",
            AccountTypeId = 9999,
            CurrencyId = 1,
            InitialBalance = 100,
            OverdraftLimit = 0,
            IsActive = true
        };

        // Act
        var response = await HttpClient.PostAsJsonAsync("/api/v1/accounts/cash", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CreateCashAccount_WithInvalidCurrencyId_ReturnsBadRequest()
    {
        // Arrange: use seeded account type, invalid currency
        var request = new CreateCashAccountRequestDto
        {
            Name = "Test Cash Account",
            Institution = "Test Bank",
            AccountTypeId = 1,
            CurrencyId = 9999,
            InitialBalance = 100,
            OverdraftLimit = 0,
            IsActive = true
        };

        // Act
        var response = await HttpClient.PostAsJsonAsync("/api/v1/accounts/cash", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CreateCashAccount_WithInvalidData_ReturnsBadRequest()
    {
        // Arrange
        var invalidRequest = new CreateCashAccountRequestDto(); // Missing required fields

        // Act
        var response = await HttpClient.PostAsJsonAsync("/api/v1/accounts/cash", invalidRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    #endregion

    #region Create Investment Account Tests

    [Fact]
    public async Task CreateInvestmentAccount_WithInvalidData_ReturnsBadRequest()
    {
        // Arrange
        var invalidRequest = new CreateInvestmentAccountRequestDto(); // Missing required fields

        // Act
        var response = await HttpClient.PostAsJsonAsync("/api/v1/accounts/investment", invalidRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    #endregion


}
