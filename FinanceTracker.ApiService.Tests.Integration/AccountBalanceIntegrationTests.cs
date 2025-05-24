using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using FinanceTracker.Data.Models;
using FinanceTracker.ApiService.Dtos;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace FinanceTracker.ApiService.Tests.Integration;

/// <summary>
/// Integration tests for account balance calculations across different scenarios
/// </summary>
public class AccountBalanceIntegrationTests : IClassFixture<FinanceTrackerWebApplicationFactory>
{
    private readonly FinanceTrackerWebApplicationFactory _factory;
    private readonly HttpClient _client;

    public AccountBalanceIntegrationTests(FinanceTrackerWebApplicationFactory factory)
    {
        _factory = factory;
        _client = _factory.CreateClient();
    }

    [Fact]
    public async Task GetCashAccount_WithNoTransactions_ReturnsOpeningBalance()
    {
        // Arrange
        using var context = _factory.GetDbContext();
        
        var currency = TestDataBuilder.CreateCurrency();
        var accountType = TestDataBuilder.CreateAccountType("Checking");
        context.Currencies.Add(currency);
        context.AccountTypes.Add(accountType);
        await context.SaveChangesAsync();

        var account = TestDataBuilder.CreateCashAccount(
            "Test Account", 
            accountType.Id, 
            currency.Id);
        context.CashAccounts.Add(account);
        await context.SaveChangesAsync();

        var openingBalance = 1500m;
        var accountPeriod = TestDataBuilder.CreateAccountPeriod(account.Id, openingBalance);
        context.AccountPeriods.Add(accountPeriod);
        await context.SaveChangesAsync();

        // Act
        var response = await _client.GetAsync($"/api/v1/accounts/{account.Id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var accountDto = await response.Content.ReadFromJsonAsync<CashAccountDto>();
        accountDto.Should().NotBeNull();
        accountDto!.CurrentBalance.Should().Be(openingBalance);
    }

    [Fact]
    public async Task GetCashAccount_WithTransactions_ReturnsCalculatedBalance()
    {
        // Arrange
        using var context = _factory.GetDbContext();
        
        // Setup basic entities
        var currency = TestDataBuilder.CreateCurrency();
        var accountType = TestDataBuilder.CreateAccountType("Checking");
        var transactionType = TestDataBuilder.CreateTransactionType("Income");
        var category = TestDataBuilder.CreateTransactionCategory("Salary");
        
        context.Currencies.Add(currency);
        context.AccountTypes.Add(accountType);
        context.TransactionTypes.Add(transactionType);
        context.TransactionCategories.Add(category);
        await context.SaveChangesAsync();

        var account = TestDataBuilder.CreateCashAccount(
            "Test Account", 
            accountType.Id, 
            currency.Id);
        context.CashAccounts.Add(account);
        await context.SaveChangesAsync();

        var openingBalance = 1000m;
        var accountPeriod = TestDataBuilder.CreateAccountPeriod(account.Id, openingBalance);
        context.AccountPeriods.Add(accountPeriod);
        await context.SaveChangesAsync();

        // Add transactions
        var transaction1 = TestDataBuilder.CreateCashTransaction(
            accountPeriod.Id, 500m, description: "Salary", 
            transactionTypeId: transactionType.Id, categoryId: category.Id);
        var transaction2 = TestDataBuilder.CreateCashTransaction(
            accountPeriod.Id, -200m, description: "Grocery", 
            transactionTypeId: transactionType.Id, categoryId: category.Id);
        
        context.CashTransactions.AddRange(transaction1, transaction2);
        await context.SaveChangesAsync();

        // Act
        var response = await _client.GetAsync($"/api/v1/accounts/{account.Id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var accountDto = await response.Content.ReadFromJsonAsync<CashAccountDto>();
        accountDto.Should().NotBeNull();
        
        // Expected: Opening Balance (1000) + Transaction1 (500) + Transaction2 (-200) = 1300
        accountDto!.CurrentBalance.Should().Be(1300m);
    }

    [Fact]
    public async Task GetInvestmentAccount_WithTransactions_ReturnsCalculatedBalance()
    {
        // Arrange
        using var context = _factory.GetDbContext();
        
        // Setup basic entities
        var currency = TestDataBuilder.CreateCurrency();
        var accountType = TestDataBuilder.CreateAccountType("Investment");
        var transactionType = TestDataBuilder.CreateTransactionType("Buy");
        var category = TestDataBuilder.CreateTransactionCategory("Investment Purchase");
        
        context.Currencies.Add(currency);
        context.AccountTypes.Add(accountType);
        context.TransactionTypes.Add(transactionType);
        context.TransactionCategories.Add(category);
        await context.SaveChangesAsync();

        var account = TestDataBuilder.CreateInvestmentAccount(
            "Test Investment Account", 
            accountType.Id, 
            currency.Id);
        context.InvestmentAccounts.Add(account);
        await context.SaveChangesAsync();

        var security = TestDataBuilder.CreateSecurity(currencyId: currency.Id);
        context.Securities.Add(security);
        await context.SaveChangesAsync();

        var openingBalance = 10000m;
        var accountPeriod = TestDataBuilder.CreateAccountPeriod(account.Id, openingBalance);
        context.AccountPeriods.Add(accountPeriod);
        await context.SaveChangesAsync();

        // Add investment transactions
        var transaction1 = TestDataBuilder.CreateInvestmentTransaction(
            accountPeriod.Id, security.Id, quantity: 10m, price: 100m,
            transactionTypeId: transactionType.Id, categoryId: category.Id);
        var transaction2 = TestDataBuilder.CreateInvestmentTransaction(
            accountPeriod.Id, security.Id, quantity: 5m, price: 150m,
            transactionTypeId: transactionType.Id, categoryId: category.Id);
        
        context.InvestmentTransactions.AddRange(transaction1, transaction2);
        await context.SaveChangesAsync();

        // Act
        var response = await _client.GetAsync($"/api/v1/accounts/{account.Id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var accountDto = await response.Content.ReadFromJsonAsync<InvestmentAccountDto>();
        accountDto.Should().NotBeNull();
        
        // Expected: Opening Balance (10000) + Transaction1 (10 * 100) + Transaction2 (5 * 150) = 11750
        accountDto!.CurrentBalance.Should().Be(11750m);
    }

    [Fact]
    public async Task GetAccount_WithClosedPeriod_OnlyCalculatesOpenPeriodBalance()
    {
        // Arrange
        using var context = _factory.GetDbContext();
        
        // Setup basic entities
        var currency = TestDataBuilder.CreateCurrency();
        var accountType = TestDataBuilder.CreateAccountType("Checking");
        var transactionType = TestDataBuilder.CreateTransactionType("Income");
        var category = TestDataBuilder.CreateTransactionCategory("Salary");
        
        context.Currencies.Add(currency);
        context.AccountTypes.Add(accountType);
        context.TransactionTypes.Add(transactionType);
        context.TransactionCategories.Add(category);
        await context.SaveChangesAsync();

        var account = TestDataBuilder.CreateCashAccount(
            "Test Account", 
            accountType.Id, 
            currency.Id);
        context.CashAccounts.Add(account);
        await context.SaveChangesAsync();

        // Create closed period with transactions
        var closedPeriod = TestDataBuilder.CreateAccountPeriod(
            account.Id, 
            openingBalance: 1000m,
            periodStart: DateOnly.FromDateTime(DateTime.Today.AddMonths(-2)),
            periodEnd: DateOnly.FromDateTime(DateTime.Today.AddMonths(-1)),
            closingBalance: 1200m,
            periodCloseDate: DateOnly.FromDateTime(DateTime.Today.AddMonths(-1)));
        context.AccountPeriods.Add(closedPeriod);
        await context.SaveChangesAsync();

        var closedPeriodTransaction = TestDataBuilder.CreateCashTransaction(
            closedPeriod.Id, 200m, description: "Old Transaction",
            transactionTypeId: transactionType.Id, categoryId: category.Id);
        context.CashTransactions.Add(closedPeriodTransaction);

        // Create open period with transactions
        var openPeriod = TestDataBuilder.CreateAccountPeriod(
            account.Id, 
            openingBalance: 1200m,
            periodStart: DateOnly.FromDateTime(DateTime.Today.AddMonths(-1)));
        context.AccountPeriods.Add(openPeriod);
        await context.SaveChangesAsync();

        var openPeriodTransaction = TestDataBuilder.CreateCashTransaction(
            openPeriod.Id, 300m, description: "New Transaction",
            transactionTypeId: transactionType.Id, categoryId: category.Id);
        context.CashTransactions.Add(openPeriodTransaction);
        await context.SaveChangesAsync();

        // Act
        var response = await _client.GetAsync($"/api/v1/accounts/{account.Id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var accountDto = await response.Content.ReadFromJsonAsync<CashAccountDto>();
        accountDto.Should().NotBeNull();
        
        // Expected: Only open period balance: Opening (1200) + New Transaction (300) = 1500
        // Closed period should not affect the calculation
        accountDto!.CurrentBalance.Should().Be(1500m);
    }

    [Fact]
    public async Task GetAllAccounts_ReturnsCorrectBalancesForAllAccountTypes()
    {
        // Arrange
        using var context = _factory.GetDbContext();
        
        // Setup basic entities
        var currency = TestDataBuilder.CreateCurrency();
        var checkingType = TestDataBuilder.CreateAccountType("Checking");
        var investmentType = TestDataBuilder.CreateAccountType("Investment");
        var transactionType = TestDataBuilder.CreateTransactionType("Income");
        var category = TestDataBuilder.CreateTransactionCategory("Test Category");
        
        context.Currencies.Add(currency);
        context.AccountTypes.AddRange(checkingType, investmentType);
        context.TransactionTypes.Add(transactionType);
        context.TransactionCategories.Add(category);
        await context.SaveChangesAsync();

        // Create cash account
        var cashAccount = TestDataBuilder.CreateCashAccount(
            "Cash Account", checkingType.Id, currency.Id);
        context.CashAccounts.Add(cashAccount);
        
        // Create investment account
        var investmentAccount = TestDataBuilder.CreateInvestmentAccount(
            "Investment Account", investmentType.Id, currency.Id);
        context.InvestmentAccounts.Add(investmentAccount);
        await context.SaveChangesAsync();

        var security = TestDataBuilder.CreateSecurity(currencyId: currency.Id);
        context.Securities.Add(security);
        await context.SaveChangesAsync();

        // Create periods and transactions for cash account
        var cashPeriod = TestDataBuilder.CreateAccountPeriod(cashAccount.Id, 1000m);
        context.AccountPeriods.Add(cashPeriod);
        await context.SaveChangesAsync();

        var cashTransaction = TestDataBuilder.CreateCashTransaction(
            cashPeriod.Id, 500m, transactionTypeId: transactionType.Id, categoryId: category.Id);
        context.CashTransactions.Add(cashTransaction);

        // Create periods and transactions for investment account
        var investmentPeriod = TestDataBuilder.CreateAccountPeriod(investmentAccount.Id, 5000m);
        context.AccountPeriods.Add(investmentPeriod);
        await context.SaveChangesAsync();

        var investmentTransaction = TestDataBuilder.CreateInvestmentTransaction(
            investmentPeriod.Id, security.Id, quantity: 10m, price: 100m,
            transactionTypeId: transactionType.Id, categoryId: category.Id);
        context.InvestmentTransactions.Add(investmentTransaction);
        await context.SaveChangesAsync();

        // Act
        var response = await _client.GetAsync("/api/v1/accounts");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var accounts = await response.Content.ReadFromJsonAsync<List<AccountBaseDto>>();
        accounts.Should().NotBeNull();
        accounts!.Should().HaveCount(2);

        var cashAccountDto = accounts.Should().ContainSingle(a => a.AccountKind == "Cash").Subject;
        cashAccountDto.CurrentBalance.Should().Be(1500m); // 1000 + 500

        var investmentAccountDto = accounts.Should().ContainSingle(a => a.AccountKind == "Investment").Subject;
        investmentAccountDto.CurrentBalance.Should().Be(6000m); // 5000 + (10 * 100)
    }

    [Fact]
    public async Task GetCashAccount_WithTransferTransaction_CalculatesBalanceCorrectly()
    {
        // Arrange
        using var context = _factory.GetDbContext();
        
        // Setup basic entities
        var currency = TestDataBuilder.CreateCurrency();
        var accountType = TestDataBuilder.CreateAccountType("Checking");
        var transactionType = TestDataBuilder.CreateTransactionType("Transfer");
        var category = TestDataBuilder.CreateTransactionCategory("Transfer");
        
        context.Currencies.Add(currency);
        context.AccountTypes.Add(accountType);
        context.TransactionTypes.Add(transactionType);
        context.TransactionCategories.Add(category);
        await context.SaveChangesAsync();

        // Create two accounts for transfer
        var sourceAccount = TestDataBuilder.CreateCashAccount(
            "Source Account", accountType.Id, currency.Id);
        var targetAccount = TestDataBuilder.CreateCashAccount(
            "Target Account", accountType.Id, currency.Id);
        context.CashAccounts.AddRange(sourceAccount, targetAccount);
        await context.SaveChangesAsync();

        // Create periods
        var sourcePeriod = TestDataBuilder.CreateAccountPeriod(sourceAccount.Id, 1000m);
        var targetPeriod = TestDataBuilder.CreateAccountPeriod(targetAccount.Id, 500m);
        context.AccountPeriods.AddRange(sourcePeriod, targetPeriod);
        await context.SaveChangesAsync();

        // Create transfer transactions (negative from source, positive to target)
        var transferOutTransaction = TestDataBuilder.CreateCashTransaction(
            sourcePeriod.Id, -300m, description: "Transfer Out",
            transactionTypeId: transactionType.Id, categoryId: category.Id,
            transferToCashAccountId: targetAccount.Id);
        var transferInTransaction = TestDataBuilder.CreateCashTransaction(
            targetPeriod.Id, 300m, description: "Transfer In",
            transactionTypeId: transactionType.Id, categoryId: category.Id);
        
        context.CashTransactions.AddRange(transferOutTransaction, transferInTransaction);
        await context.SaveChangesAsync();

        // Act - Check source account
        var sourceResponse = await _client.GetAsync($"/api/v1/accounts/{sourceAccount.Id}");
        var targetResponse = await _client.GetAsync($"/api/v1/accounts/{targetAccount.Id}");

        // Assert
        sourceResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        targetResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var sourceAccountDto = await sourceResponse.Content.ReadFromJsonAsync<CashAccountDto>();
        var targetAccountDto = await targetResponse.Content.ReadFromJsonAsync<CashAccountDto>();

        sourceAccountDto.Should().NotBeNull();
        targetAccountDto.Should().NotBeNull();

        sourceAccountDto!.CurrentBalance.Should().Be(700m); // 1000 - 300
        targetAccountDto!.CurrentBalance.Should().Be(800m); // 500 + 300
    }
}
