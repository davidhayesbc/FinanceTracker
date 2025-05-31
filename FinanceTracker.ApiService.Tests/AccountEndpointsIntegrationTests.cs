using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using FluentAssertions;
using FinanceTracker.ApiService.Dtos;
using FinanceTracker.ApiService.Tests.Infrastructure;
using FinanceTracker.Data.Models;

namespace FinanceTracker.ApiService.Tests;

/// <summary>
/// Integration tests for Account endpoints (/accounts)
/// Tests all account-related API endpoints including cash and investment accounts
/// </summary>
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

    #region Integration Tests with Valid Data

    // Note: These tests require valid reference data (AccountType, Currency) to be present
    // They are commented out for now and will be implemented once we have proper test data seeding

    /*
    [Fact]
    public async Task CreateCashAccount_WithValidData_ReturnsCreated()
    {
        // Arrange
        var client = await GetApiClientAsync();
        
        // First create the required reference data
        var accountType = TestDataSeeder.CreateTestAccountType("Checking");
        var currency = TestDataSeeder.CreateTestCurrency("USD", "$");
        
        // Seed the database with reference data
        await SeedReferenceDataAsync(accountType, currency);
        
        var request = TestDataSeeder.CreateTestCashAccountRequest(
            name: "Test Checking Account",
            accountTypeId: accountType.Id,
            currencyId: currency.Id,
            openingBalance: 1000.00m);

        // Act
        var response = await client.PostAsJsonAsync("/accounts/cash", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var createdAccount = await response.Content.ReadFromJsonAsync<CashAccountDto>();
        createdAccount.Should().NotBeNull();
        createdAccount!.Name.Should().Be(request.Name);
        createdAccount.Institution.Should().Be(request.Institution);
        createdAccount.InitialBalance.Should().Be(request.InitialBalance);
        createdAccount.OverdraftLimit.Should().Be(request.OverdraftLimit);
    }

    [Fact]
    public async Task CreateInvestmentAccount_WithValidData_ReturnsCreated()
    {
        // Arrange
        var client = await GetApiClientAsync();
        
        // First create the required reference data
        var accountType = TestDataSeeder.CreateTestAccountType("Investment");
        var currency = TestDataSeeder.CreateTestCurrency("USD", "$");
        
        // Seed the database with reference data
        await SeedReferenceDataAsync(accountType, currency);
        
        var request = TestDataSeeder.CreateTestInvestmentAccountRequest(
            name: "Test Investment Account",
            accountTypeId: accountType.Id,
            currencyId: currency.Id,
            openingBalance: 5000.00m);

        // Act
        var response = await client.PostAsJsonAsync("/accounts/investment", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var createdAccount = await response.Content.ReadFromJsonAsync<InvestmentAccountDto>();
        createdAccount.Should().NotBeNull();
        createdAccount!.Name.Should().Be(request.Name);
        createdAccount.Institution.Should().Be(request.Institution);
        createdAccount.InitialBalance.Should().Be(request.InitialBalance);
        createdAccount.BrokerAccountNumber.Should().Be(request.BrokerAccountNumber);
    }

    [Fact]
    public async Task GetAllAccounts_WithAccountsExist_ReturnsAccountList()
    {
        // Arrange
        var client = await GetApiClientAsync();
        
        // Create and seed test accounts
        await CreateTestAccountsAsync();

        // Act
        var response = await client.GetAsync("/accounts");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var accounts = await response.Content.ReadFromJsonAsync<List<AccountBaseDto>>();
        accounts.Should().NotBeNull();
        accounts.Should().NotBeEmpty();
        accounts.Should().HaveCountGreaterThan(0);
    }

    [Fact]
    public async Task GetAccountById_WithValidId_ReturnsAccount()
    {
        // Arrange
        var client = await GetApiClientAsync();
        
        // Create a test account and get its ID
        var accountId = await CreateTestCashAccountAsync();

        // Act
        var response = await client.GetAsync($"/accounts/{accountId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var account = await response.Content.ReadFromJsonAsync<AccountBaseDto>();
        account.Should().NotBeNull();
        account!.Id.Should().Be(accountId);
    }

    [Fact]
    public async Task GetAccountTransactions_WithValidAccountId_ReturnsTransactionList()
    {
        // Arrange
        var client = await GetApiClientAsync();
        
        // Create a test account
        var accountId = await CreateTestCashAccountAsync();

        // Act
        var response = await client.GetAsync($"/accounts/{accountId}/transactions");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var transactions = await response.Content.ReadFromJsonAsync<List<TransactionBaseDto>>();
        transactions.Should().NotBeNull();
        // Note: Initially empty since we haven't created transactions
    }
    */

    #endregion

    #region Helper Methods

    /// <summary>
    /// Seeds the database with reference data required for account creation
    /// </summary>
    private async Task SeedReferenceDataAsync(AccountType accountType, Currency currency)
    {
        // TODO: Implement database seeding logic
        // This would typically involve getting a database context and adding the entities
        await Task.CompletedTask;
    }

    /// <summary>
    /// Creates test accounts for integration testing
    /// </summary>
    private async Task CreateTestAccountsAsync()
    {
        // TODO: Implement test account creation
        await Task.CompletedTask;
    }

    /// <summary>
    /// Creates a test cash account and returns its ID
    /// </summary>
    private async Task<int> CreateTestCashAccountAsync()
    {
        // TODO: Implement test cash account creation
        await Task.CompletedTask;
        return 1;
    }

    #endregion
}
