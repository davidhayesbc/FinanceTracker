using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using FluentAssertions;
using FinanceTracker.Data.Models;
using FinanceTracker.ApiService.Dtos;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace FinanceTracker.ApiService.Tests.Integration;

/// <summary>
/// Integration tests for transaction endpoints and their impact on account balances
/// </summary>
public class TransactionIntegrationTests : IClassFixture<FinanceTrackerWebApplicationFactory>
{
    private readonly FinanceTrackerWebApplicationFactory _factory;
    private readonly HttpClient _client;

    public TransactionIntegrationTests(FinanceTrackerWebApplicationFactory factory)
    {
        _factory = factory;
        _client = _factory.CreateClient();
    }

    [Fact]
    public async Task CreateCashTransaction_UpdatesAccountBalance()
    {
        // Arrange
        using var context = _factory.GetDbContext();
        var (account, accountPeriod) = await SetupBasicCashAccountAsync(context);
        
        var createTransactionDto = new CreateCashTransactionRequestDto
        {
            TransactionDate = DateOnly.FromDateTime(DateTime.Today),
            Description = "Test Income",
            Amount = 500m,
            TransactionTypeId = 1,
            AccountPeriodId = accountPeriod.Id
        };

        // Get initial balance
        var initialResponse = await _client.GetAsync($"/api/v1/accounts/{account.Id}");
        var initialAccount = await initialResponse.Content.ReadFromJsonAsync<CashAccountDto>();
        var initialBalance = initialAccount!.CurrentBalance;

        // Act - Create transaction
        var createResponse = await _client.PostAsJsonAsync("/api/v1/transactions/cash", createTransactionDto);

        // Assert transaction creation
        createResponse.StatusCode.Should().Be(HttpStatusCode.Created);

        // Verify balance updated
        var updatedResponse = await _client.GetAsync($"/api/v1/accounts/{account.Id}");
        var updatedAccount = await updatedResponse.Content.ReadFromJsonAsync<CashAccountDto>();
        updatedAccount!.CurrentBalance.Should().Be(initialBalance + 500m);
    }

    [Fact]
    public async Task CreateInvestmentTransaction_UpdatesAccountBalance()
    {
        // Arrange
        using var context = _factory.GetDbContext();
        var (account, accountPeriod, security) = await SetupBasicInvestmentAccountAsync(context);
        
        var createTransactionDto = new CreateInvestmentTransactionRequestDto
        {
            TransactionDate = DateOnly.FromDateTime(DateTime.Today),
            Description = "Buy Stock",
            Quantity = 10m,
            Price = 150m,
            SecurityId = security.Id,
            TransactionTypeId = 1,
            AccountPeriodId = accountPeriod.Id
        };

        // Get initial balance
        var initialResponse = await _client.GetAsync($"/api/v1/accounts/{account.Id}");
        var initialAccount = await initialResponse.Content.ReadFromJsonAsync<InvestmentAccountDto>();
        var initialBalance = initialAccount!.CurrentBalance;

        // Act - Create transaction
        var createResponse = await _client.PostAsJsonAsync("/api/v1/transactions/investment", createTransactionDto);

        // Assert transaction creation
        createResponse.StatusCode.Should().Be(HttpStatusCode.Created);

        // Verify balance updated
        var updatedResponse = await _client.GetAsync($"/api/v1/accounts/{account.Id}");
        var updatedAccount = await updatedResponse.Content.ReadFromJsonAsync<InvestmentAccountDto>();
        updatedAccount!.CurrentBalance.Should().Be(initialBalance + (10m * 150m)); // quantity * price
    }

    [Fact]
    public async Task CreateTransferTransaction_UpdatesBothAccountBalances()
    {
        // Arrange
        using var context = _factory.GetDbContext();
        var (sourceAccount, sourcePeriod) = await SetupBasicCashAccountAsync(context, "Source Account", 1000m);
        var (targetAccount, targetPeriod) = await SetupAdditionalCashAccountAsync(context, "Target Account", 500m);
        
        var transferAmount = 300m;

        // Create transfer out transaction (from source)
        var transferOutDto = new CreateCashTransactionRequestDto
        {
            TransactionDate = DateOnly.FromDateTime(DateTime.Today),
            Description = "Transfer to Target Account",
            Amount = -transferAmount, // Negative for outgoing transfer
            TransferToCashAccountId = targetAccount.Id,
            TransactionTypeId = 1,
            AccountPeriodId = sourcePeriod.Id
        };

        // Create transfer in transaction (to target)
        var transferInDto = new CreateCashTransactionRequestDto
        {
            TransactionDate = DateOnly.FromDateTime(DateTime.Today),
            Description = "Transfer from Source Account",
            Amount = transferAmount, // Positive for incoming transfer
            TransactionTypeId = 1,
            AccountPeriodId = targetPeriod.Id
        };

        // Act - Create both transfer transactions
        var transferOutResponse = await _client.PostAsJsonAsync("/api/v1/transactions/cash", transferOutDto);
        var transferInResponse = await _client.PostAsJsonAsync("/api/v1/transactions/cash", transferInDto);

        // Assert both transactions created successfully
        transferOutResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        transferInResponse.StatusCode.Should().Be(HttpStatusCode.Created);

        // Verify balances updated correctly
        var sourceResponse = await _client.GetAsync($"/api/v1/accounts/{sourceAccount.Id}");
        var targetResponse = await _client.GetAsync($"/api/v1/accounts/{targetAccount.Id}");

        var sourceAccountDto = await sourceResponse.Content.ReadFromJsonAsync<CashAccountDto>();
        var targetAccountDto = await targetResponse.Content.ReadFromJsonAsync<CashAccountDto>();

        sourceAccountDto!.CurrentBalance.Should().Be(700m); // 1000 - 300
        targetAccountDto!.CurrentBalance.Should().Be(800m); // 500 + 300
    }

    [Fact]
    public async Task GetAccountTransactions_ReturnsOnlyCurrentPeriodTransactions()
    {
        // Arrange
        using var context = _factory.GetDbContext();
        var (account, currentPeriod) = await SetupBasicCashAccountAsync(context);

        // Create a closed period with a transaction
        var closedPeriod = TestDataBuilder.CreateAccountPeriod(
            account.Id, 
            openingBalance: 800m,
            periodStart: DateOnly.FromDateTime(DateTime.Today.AddMonths(-2)),
            periodEnd: DateOnly.FromDateTime(DateTime.Today.AddMonths(-1)),
            closingBalance: 900m,
            periodCloseDate: DateOnly.FromDateTime(DateTime.Today.AddMonths(-1)));
        context.AccountPeriods.Add(closedPeriod);
        await context.SaveChangesAsync();

        var closedPeriodTransaction = TestDataBuilder.CreateCashTransaction(
            closedPeriod.Id, 100m, description: "Old Transaction");
        context.CashTransactions.Add(closedPeriodTransaction);

        // Create current period transactions
        var currentTransaction1 = TestDataBuilder.CreateCashTransaction(
            currentPeriod.Id, 200m, description: "Current Transaction 1");
        var currentTransaction2 = TestDataBuilder.CreateCashTransaction(
            currentPeriod.Id, -50m, description: "Current Transaction 2");
        context.CashTransactions.AddRange(currentTransaction1, currentTransaction2);
        await context.SaveChangesAsync();

        // Act
        var response = await _client.GetAsync($"/api/v1/accounts/{account.Id}/transactions");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var transactions = await response.Content.ReadFromJsonAsync<List<CashTransactionDto>>();
        transactions.Should().NotBeNull();
        transactions!.Should().HaveCount(2); // Only current period transactions
        transactions.Should().Contain(t => t.Description == "Current Transaction 1");
        transactions.Should().Contain(t => t.Description == "Current Transaction 2");
        transactions.Should().NotContain(t => t.Description == "Old Transaction");
    }

    [Fact]
    public async Task CreateTransaction_WithInvalidAccountPeriod_ReturnsBadRequest()
    {
        // Arrange
        var createTransactionDto = new CreateCashTransactionRequestDto
        {
            TransactionDate = DateOnly.FromDateTime(DateTime.Today),
            Description = "Invalid Transaction",
            Amount = 100m,
            TransactionTypeId = 1,
            AccountPeriodId = 999 // Non-existent period
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/v1/transactions/cash", createTransactionDto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CreateCashTransaction_WithSplits_CalculatesBalanceCorrectly()
    {
        // Arrange
        using var context = _factory.GetDbContext();
        var (account, accountPeriod) = await SetupBasicCashAccountAsync(context);

        // Create additional categories for splits
        var category1 = TestDataBuilder.CreateTransactionCategory("Groceries");
        var category2 = TestDataBuilder.CreateTransactionCategory("Gas");
        context.TransactionCategories.AddRange(category1, category2);
        await context.SaveChangesAsync();

        // Create main transaction
        var createTransactionDto = new CreateCashTransactionRequestDto
        {
            TransactionDate = DateOnly.FromDateTime(DateTime.Today),
            Description = "Shopping Trip",
            Amount = -150m,
            TransactionTypeId = 1,
            AccountPeriodId = accountPeriod.Id
        };

        var createResponse = await _client.PostAsJsonAsync("/api/v1/transactions/cash", createTransactionDto);
        createResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        
        var createdTransaction = await createResponse.Content.ReadFromJsonAsync<CashTransaction>();

        // Create transaction splits
        var split1Dto = new CreateTransactionSplitRequestDto
        {
            CashTransactionId = createdTransaction!.Id,
            CategoryId = category1.Id,
            Amount = -100m
        };

        var split2Dto = new CreateTransactionSplitRequestDto
        {
            CashTransactionId = createdTransaction.Id,
            CategoryId = category2.Id,
            Amount = -50m
        };

        await _client.PostAsJsonAsync("/api/v1/transactionSplits", split1Dto);
        await _client.PostAsJsonAsync("/api/v1/transactionSplits", split2Dto);

        // Act - Get updated account balance
        var accountResponse = await _client.GetAsync($"/api/v1/accounts/{account.Id}");

        // Assert
        var accountDto = await accountResponse.Content.ReadFromJsonAsync<CashAccountDto>();
        accountDto!.CurrentBalance.Should().Be(850m); // 1000 - 150

        // Verify splits were created
        var splitsResponse = await _client.GetAsync("/api/v1/transactionSplits");
        var splits = await splitsResponse.Content.ReadFromJsonAsync<List<TransactionSplitDto>>();
        splits.Should().HaveCount(2);
        splits!.Sum(s => s.Amount).Should().Be(-150m); // Should equal transaction amount
    }

    [Fact]
    public async Task CreateMultipleTransactions_InSameAccount_CalculatesRunningBalance()
    {
        // Arrange
        using var context = _factory.GetDbContext();
        var (account, accountPeriod) = await SetupBasicCashAccountAsync(context, "Test Account", 1000m);

        var transactions = new[]
        {
            new CreateCashTransactionRequestDto
            {
                TransactionDate = DateOnly.FromDateTime(DateTime.Today.AddDays(-3)),
                Description = "Salary",
                Amount = 2000m,
                TransactionTypeId = 1,
                AccountPeriodId = accountPeriod.Id
            },
            new CreateCashTransactionRequestDto
            {
                TransactionDate = DateOnly.FromDateTime(DateTime.Today.AddDays(-2)),
                Description = "Rent",
                Amount = -800m,
                TransactionTypeId = 1,
                AccountPeriodId = accountPeriod.Id
            },
            new CreateCashTransactionRequestDto
            {
                TransactionDate = DateOnly.FromDateTime(DateTime.Today.AddDays(-1)),
                Description = "Groceries",
                Amount = -150m,
                TransactionTypeId = 1,
                AccountPeriodId = accountPeriod.Id
            }
        };

        // Act - Create all transactions
        foreach (var transaction in transactions)
        {
            var response = await _client.PostAsJsonAsync("/api/v1/transactions/cash", transaction);
            response.StatusCode.Should().Be(HttpStatusCode.Created);
        }

        // Get final balance
        var accountResponse = await _client.GetAsync($"/api/v1/accounts/{account.Id}");

        // Assert
        var accountDto = await accountResponse.Content.ReadFromJsonAsync<CashAccountDto>();
        var expectedBalance = 1000m + 2000m - 800m - 150m; // 2050
        accountDto!.CurrentBalance.Should().Be(expectedBalance);

        // Verify all transactions are present
        var transactionsResponse = await _client.GetAsync($"/api/v1/accounts/{account.Id}/transactions");
        var accountTransactions = await transactionsResponse.Content.ReadFromJsonAsync<List<CashTransactionDto>>();
        accountTransactions.Should().HaveCount(3);
    }

    #region Helper Methods

    private async Task<(CashAccount account, AccountPeriod period)> SetupBasicCashAccountAsync(
        FinanceTackerDbContext context, 
        string accountName = "Test Account", 
        decimal openingBalance = 1000m)
    {
        // Setup basic entities if they don't exist
        if (!await context.Currencies.AnyAsync())
        {
            var currency = TestDataBuilder.CreateCurrency();
            var accountType = TestDataBuilder.CreateAccountType("Checking");
            var transactionType = TestDataBuilder.CreateTransactionType("Income");
            var category = TestDataBuilder.CreateTransactionCategory("General");

            context.Currencies.Add(currency);
            context.AccountTypes.Add(accountType);
            context.TransactionTypes.Add(transactionType);
            context.TransactionCategories.Add(category);
            await context.SaveChangesAsync();
        }

        var existingCurrency = await context.Currencies.FirstAsync();
        var existingAccountType = await context.AccountTypes.FirstAsync();

        var account = TestDataBuilder.CreateCashAccount(
            accountName, existingAccountType.Id, existingCurrency.Id);
        context.CashAccounts.Add(account);
        await context.SaveChangesAsync();

        var accountPeriod = TestDataBuilder.CreateAccountPeriod(account.Id, openingBalance);
        context.AccountPeriods.Add(accountPeriod);
        await context.SaveChangesAsync();

        return (account, accountPeriod);
    }

    private async Task<(CashAccount account, AccountPeriod period)> SetupAdditionalCashAccountAsync(
        FinanceTackerDbContext context, 
        string accountName, 
        decimal openingBalance = 500m)
    {
        var existingCurrency = await context.Currencies.FirstAsync();
        var existingAccountType = await context.AccountTypes.FirstAsync();

        var account = TestDataBuilder.CreateCashAccount(
            accountName, existingAccountType.Id, existingCurrency.Id);
        context.CashAccounts.Add(account);
        await context.SaveChangesAsync();

        var accountPeriod = TestDataBuilder.CreateAccountPeriod(account.Id, openingBalance);
        context.AccountPeriods.Add(accountPeriod);
        await context.SaveChangesAsync();

        return (account, accountPeriod);
    }

    private async Task<(InvestmentAccount account, AccountPeriod period, Security security)> SetupBasicInvestmentAccountAsync(
        FinanceTackerDbContext context,
        string accountName = "Test Investment Account",
        decimal openingBalance = 5000m)
    {
        // Setup basic entities if they don't exist
        if (!await context.Currencies.AnyAsync())
        {
            var currency = TestDataBuilder.CreateCurrency();
            var accountType = TestDataBuilder.CreateAccountType("Investment");
            var transactionType = TestDataBuilder.CreateTransactionType("Buy");
            var category = TestDataBuilder.CreateTransactionCategory("Investment");

            context.Currencies.Add(currency);
            context.AccountTypes.Add(accountType);
            context.TransactionTypes.Add(transactionType);
            context.TransactionCategories.Add(category);
            await context.SaveChangesAsync();
        }

        var existingCurrency = await context.Currencies.FirstAsync();
        var investmentAccountType = await context.AccountTypes.FirstOrDefaultAsync(at => at.Type == "Investment");
        if (investmentAccountType == null)
        {
            investmentAccountType = TestDataBuilder.CreateAccountType("Investment");
            context.AccountTypes.Add(investmentAccountType);
            await context.SaveChangesAsync();
        }

        var account = TestDataBuilder.CreateInvestmentAccount(
            accountName, investmentAccountType.Id, existingCurrency.Id);
        context.InvestmentAccounts.Add(account);
        await context.SaveChangesAsync();

        var security = TestDataBuilder.CreateSecurity(currencyId: existingCurrency.Id);
        context.Securities.Add(security);
        await context.SaveChangesAsync();

        var accountPeriod = TestDataBuilder.CreateAccountPeriod(account.Id, openingBalance);
        context.AccountPeriods.Add(accountPeriod);
        await context.SaveChangesAsync();

        return (account, accountPeriod, security);
    }

    #endregion
}
