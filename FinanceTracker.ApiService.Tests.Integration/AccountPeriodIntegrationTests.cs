using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using FinanceTracker.Data.Models;
using FinanceTracker.ApiService.Dtos;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace FinanceTracker.ApiService.Tests.Integration;

/// <summary>
/// Integration tests for account period operations and their impact on account balances
/// </summary>
public class AccountPeriodIntegrationTests : IClassFixture<FinanceTrackerWebApplicationFactory>
{
    private readonly FinanceTrackerWebApplicationFactory _factory;
    private readonly HttpClient _client;

    public AccountPeriodIntegrationTests(FinanceTrackerWebApplicationFactory factory)
    {
        _factory = factory;
        _client = _factory.CreateClient();
    }

    [Fact]
    public async Task CreateAccountPeriod_WithTransactions_CalculatesCorrectOpeningBalance()
    {
        // Arrange
        using var context = _factory.GetDbContext();
        var (account, initialPeriod) = await SetupCashAccountWithTransactionsAsync(context);

        // Add some transactions to the initial period
        var transaction1 = TestDataBuilder.CreateCashTransaction(initialPeriod.Id, 500m, description: "Income");
        var transaction2 = TestDataBuilder.CreateCashTransaction(initialPeriod.Id, -200m, description: "Expense");
        context.CashTransactions.AddRange(transaction1, transaction2);
        await context.SaveChangesAsync();

        // Get the initial period's ending balance
        var expectedEndingBalance = initialPeriod.OpeningBalance + 500m - 200m; // 1000 + 500 - 200 = 1300

        // Act - Close the current period and create a new one
        initialPeriod.PeriodCloseDate = DateOnly.FromDateTime(DateTime.Today);
        var newPeriod = TestDataBuilder.CreateAccountPeriod(
            account.Id,
            expectedEndingBalance, // New period should start with previous period's ending balance
            DateOnly.FromDateTime(DateTime.Today.AddDays(1))
        );
        context.AccountPeriods.Add(newPeriod);
        await context.SaveChangesAsync();

        // Assert - Verify the new period has correct opening balance
        var accountResponse = await _client.GetAsync($"/api/v1/accounts/{account.Id}");
        var accountDto = await accountResponse.Content.ReadFromJsonAsync<CashAccountDto>();

        accountDto.Should().NotBeNull();
        accountDto!.CurrentBalance.Should().Be(expectedEndingBalance); // Should equal the new period's opening balance
    }

    [Fact]
    public async Task CloseAccountPeriod_CalculatesCorrectFinalBalance()
    {
        // Arrange
        using var context = _factory.GetDbContext();
        var (account, accountPeriod) = await SetupCashAccountWithTransactionsAsync(context);

        // Add multiple transactions throughout the period
        var transactions = new[]
        {
            TestDataBuilder.CreateCashTransaction(accountPeriod.Id, 1000m, description: "Salary"),
            TestDataBuilder.CreateCashTransaction(accountPeriod.Id, -300m, description: "Rent"),
            TestDataBuilder.CreateCashTransaction(accountPeriod.Id, -150m, description: "Groceries"),
            TestDataBuilder.CreateCashTransaction(accountPeriod.Id, 200m, description: "Refund"),
            TestDataBuilder.CreateCashTransaction(accountPeriod.Id, -75m, description: "Utilities")
        };
        context.CashTransactions.AddRange(transactions);
        await context.SaveChangesAsync();

        var expectedFinalBalance = accountPeriod.OpeningBalance + 1000m - 300m - 150m + 200m - 75m; // 1000 + 675 = 1675

        // Act - Close the period
        accountPeriod.PeriodCloseDate = DateOnly.FromDateTime(DateTime.Today);
        await context.SaveChangesAsync();

        // Assert - Verify the closed period has correct final balance
        var closedPeriodBalance = accountPeriod.OpeningBalance +
            context.CashTransactions.Where(t => t.AccountPeriodId == accountPeriod.Id).Sum(t => t.Amount);

        closedPeriodBalance.Should().Be(expectedFinalBalance);
    }

    [Fact]
    public async Task CreateNewPeriod_AfterClosingPrevious_CarriesForwardBalance()
    {
        // Arrange
        using var context = _factory.GetDbContext();
        var (account, period1) = await SetupCashAccountWithTransactionsAsync(context, "Test Account", 500m);

        // Add transactions to first period
        var period1Transactions = new[]
        {
            TestDataBuilder.CreateCashTransaction(period1.Id, 400m, description: "Income Period 1"),
            TestDataBuilder.CreateCashTransaction(period1.Id, -150m, description: "Expense Period 1")
        };
        context.CashTransactions.AddRange(period1Transactions);
        await context.SaveChangesAsync();

        var period1EndingBalance = 500m + 400m - 150m; // 750

        // Act - Close first period and create second period
        period1.PeriodCloseDate = DateOnly.FromDateTime(DateTime.Today.AddDays(-1));
        var period2 = TestDataBuilder.CreateAccountPeriod(
            account.Id,
            period1EndingBalance,
            DateOnly.FromDateTime(DateTime.Today)
        );
        context.AccountPeriods.Add(period2);
        await context.SaveChangesAsync();

        // Add transactions to second period
        var period2Transactions = new[]
        {
            TestDataBuilder.CreateCashTransaction(period2.Id, 300m, description: "Income Period 2"),
            TestDataBuilder.CreateCashTransaction(period2.Id, -100m, description: "Expense Period 2")
        };
        context.CashTransactions.AddRange(period2Transactions);
        await context.SaveChangesAsync();

        // Assert - Verify both periods have correct balances
        var accountResponse = await _client.GetAsync($"/api/v1/accounts/{account.Id}");
        var accountDto = await accountResponse.Content.ReadFromJsonAsync<CashAccountDto>();

        // Current balance should reflect period 2's transactions
        var expectedCurrentBalance = period1EndingBalance + 300m - 100m; // 750 + 200 = 950
        accountDto!.CurrentBalance.Should().Be(expectedCurrentBalance);

        // Verify period 1 is closed and period 2 is open
        var periods = await context.AccountPeriods
            .Where(ap => ap.AccountId == account.Id)
            .OrderBy(ap => ap.PeriodStart)
            .ToListAsync();

        periods.Should().HaveCount(2);
        periods[0].PeriodCloseDate.Should().NotBeNull(); // First period should be closed
        periods[1].PeriodCloseDate.Should().BeNull(); // Second period should be open
        periods[1].OpeningBalance.Should().Be(period1EndingBalance); // Second period starts with first period's ending balance
    }

    [Fact]
    public async Task GetAccountTransactions_WithMultiplePeriods_ReturnsOnlyCurrentPeriodTransactions()
    {
        // Arrange
        using var context = _factory.GetDbContext();
        var (account, period1) = await SetupCashAccountWithTransactionsAsync(context);

        // Add transactions to first period
        var period1Transaction = TestDataBuilder.CreateCashTransaction(period1.Id, 100m, description: "Period 1 Transaction");
        context.CashTransactions.Add(period1Transaction);
        await context.SaveChangesAsync();

        // Close first period and create second period
        period1.PeriodCloseDate = DateOnly.FromDateTime(DateTime.Today.AddDays(-1));
        var period2 = TestDataBuilder.CreateAccountPeriod(
            account.Id,
            1100m, // 1000 + 100
            DateOnly.FromDateTime(DateTime.Today)
        );
        context.AccountPeriods.Add(period2);
        await context.SaveChangesAsync();

        // Add transactions to second period
        var period2Transaction1 = TestDataBuilder.CreateCashTransaction(period2.Id, 200m, description: "Period 2 Transaction 1");
        var period2Transaction2 = TestDataBuilder.CreateCashTransaction(period2.Id, -50m, description: "Period 2 Transaction 2");
        context.CashTransactions.AddRange(period2Transaction1, period2Transaction2);
        await context.SaveChangesAsync();

        // Act
        var response = await _client.GetAsync($"/api/v1/accounts/{account.Id}/transactions");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var transactions = await response.Content.ReadFromJsonAsync<List<CashTransactionDto>>();

        transactions.Should().NotBeNull();
        transactions!.Should().HaveCount(2); // Only current period transactions
        transactions.Should().Contain(t => t.Description == "Period 2 Transaction 1");
        transactions.Should().Contain(t => t.Description == "Period 2 Transaction 2");
        transactions.Should().NotContain(t => t.Description == "Period 1 Transaction");
    }

    [Fact]
    public async Task CreateMultiplePeriods_ForSameAccount_MaintainsBalanceContinuity()
    {
        // Arrange
        using var context = _factory.GetDbContext();
        var (account, period1) = await SetupCashAccountWithTransactionsAsync(context, "Multi Period Account", 1000m);

        var periods = new List<AccountPeriod> { period1 };
        var expectedBalances = new List<decimal> { 1000m };

        // Create 3 periods with transactions
        for (int i = 0; i < 3; i++)
        {
            var currentPeriod = periods[i];

            // Add transactions to current period
            var income = (i + 1) * 500m; // 500, 1000, 1500
            var expense = (i + 1) * 200m; // 200, 400, 600

            var incomeTransaction = TestDataBuilder.CreateCashTransaction(currentPeriod.Id, income, description: $"Income Period {i + 1}");
            var expenseTransaction = TestDataBuilder.CreateCashTransaction(currentPeriod.Id, -expense, description: $"Expense Period {i + 1}");
            context.CashTransactions.AddRange(incomeTransaction, expenseTransaction);
            await context.SaveChangesAsync();

            var periodEndingBalance = expectedBalances[i] + income - expense;
            expectedBalances.Add(periodEndingBalance);

            // Close current period and create next period (except for the last iteration)
            if (i < 2)
            {
                currentPeriod.PeriodCloseDate = DateOnly.FromDateTime(DateTime.Today.AddDays(-(2 - i)));
                var nextPeriod = TestDataBuilder.CreateAccountPeriod(
                    account.Id,
                    periodEndingBalance,
                    DateOnly.FromDateTime(DateTime.Today.AddDays(-(1 - i)))
                );
                context.AccountPeriods.Add(nextPeriod);
                periods.Add(nextPeriod);
                await context.SaveChangesAsync();
            }
        }

        // Act - Get final account balance
        var accountResponse = await _client.GetAsync($"/api/v1/accounts/{account.Id}");

        // Assert
        var accountDto = await accountResponse.Content.ReadFromJsonAsync<CashAccountDto>();
        accountDto.Should().NotBeNull();

        // Final balance should be the last expected balance
        var finalExpectedBalance = expectedBalances.Last();
        accountDto!.CurrentBalance.Should().Be(finalExpectedBalance);

        // Verify all periods exist and have correct properties
        var allPeriods = await context.AccountPeriods
            .Where(ap => ap.AccountId == account.Id)
            .OrderBy(ap => ap.PeriodStart)
            .ToListAsync();

        allPeriods.Should().HaveCount(4); // Original + 3 additional periods

        // First 3 periods should be closed, last should be open
        for (int i = 0; i < 3; i++)
        {
            allPeriods[i].PeriodCloseDate.Should().NotBeNull($"Period {i + 1} should be closed");
        }
        allPeriods[3].PeriodCloseDate.Should().BeNull("Last period should be open");

        // Verify balance continuity between periods
        for (int i = 1; i < allPeriods.Count; i++)
        {
            var previousPeriodBalance = allPeriods[i - 1].OpeningBalance +
                context.CashTransactions.Where(t => t.AccountPeriodId == allPeriods[i - 1].Id).Sum(t => t.Amount);

            allPeriods[i].OpeningBalance.Should().Be(previousPeriodBalance,
                $"Period {i + 1} opening balance should equal Period {i} ending balance");
        }
    }

    [Fact]
    public async Task InvestmentAccountPeriod_WithTransactions_CalculatesCorrectBalance()
    {
        // Arrange
        using var context = _factory.GetDbContext();
        var (account, accountPeriod, security) = await SetupInvestmentAccountWithTransactionsAsync(context);

        // Add investment transactions
        var buyTransaction = TestDataBuilder.CreateInvestmentTransaction(
            accountPeriod.Id, security.Id, 10m, 100m, description: "Buy Stock");
        var sellTransaction = TestDataBuilder.CreateInvestmentTransaction(
            accountPeriod.Id, security.Id, -5m, 110m, description: "Sell Stock");

        context.InvestmentTransactions.AddRange(buyTransaction, sellTransaction);
        await context.SaveChangesAsync();

        // Act
        var accountResponse = await _client.GetAsync($"/api/v1/accounts/{account.Id}");

        // Assert
        var accountDto = await accountResponse.Content.ReadFromJsonAsync<InvestmentAccountDto>();
        accountDto.Should().NotBeNull();

        // Expected balance: opening (1000) + (10 * 100) + (-5 * 110) = 1000 + 1000 - 550 = 1450
        var expectedBalance = 1000m + (10m * 100m) + (-5m * 110m);
        accountDto!.CurrentBalance.Should().Be(expectedBalance);
    }

    #region Helper Methods

    private async Task<(CashAccount account, AccountPeriod period)> SetupCashAccountWithTransactionsAsync(
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

    private async Task<(InvestmentAccount account, AccountPeriod period, Security security)> SetupInvestmentAccountWithTransactionsAsync(
        FinanceTackerDbContext context,
        string accountName = "Test Investment Account",
        decimal openingBalance = 1000m)
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
        var existingAccountType = await context.AccountTypes.FirstAsync();

        var security = TestDataBuilder.CreateSecurity(symbol: "AAPL", currencyId: existingCurrency.Id);
        context.Securities.Add(security);
        await context.SaveChangesAsync();

        var account = TestDataBuilder.CreateInvestmentAccount(
            accountName, existingAccountType.Id, existingCurrency.Id);
        context.InvestmentAccounts.Add(account);
        await context.SaveChangesAsync();

        var accountPeriod = TestDataBuilder.CreateAccountPeriod(account.Id, openingBalance);
        context.AccountPeriods.Add(accountPeriod);
        await context.SaveChangesAsync();

        return (account, accountPeriod, security);
    }

    #endregion
}
