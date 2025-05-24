using System.Net;
using System.Net.Http.Json;
using FinanceTracker.Data.Models;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using Microsoft.EntityFrameworkCore;

namespace FinanceTracker.ApiService.Tests.Integration;

public class RecurringTransactionIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;

    public RecurringTransactionIntegrationTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _client = _factory.CreateClient();
    }

    [Fact]
    public async Task GetRecurringTransactions_ReturnsOkWithEmptyList_WhenNoRecurringTransactionsExist()
    {
        // Arrange
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<FinanceTackerDbContext>();
        await TestDataBuilder.CleanDatabase(context);

        // Act
        var response = await _client.GetAsync("/recurringTransactions");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var recurringTransactions = await response.Content.ReadFromJsonAsync<List<RecurringTransaction>>();
        Assert.NotNull(recurringTransactions);
        Assert.Empty(recurringTransactions);
    }

    [Fact]
    public async Task GetRecurringTransactions_ReturnsOkWithRecurringTransactions_WhenRecurringTransactionsExist()
    {
        // Arrange
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<FinanceTackerDbContext>();
        await TestDataBuilder.CleanDatabase(context);

        var account = TestDataBuilder.CreateCashAccount();
        var transactionType = TestDataBuilder.CreateTransactionType();
        var category = TestDataBuilder.CreateTransactionCategory();
        
        context.CashAccounts.Add(account);
        context.TransactionTypes.Add(transactionType);
        context.TransactionCategories.Add(category);
        await context.SaveChangesAsync();

        var recurringTransaction = TestDataBuilder.CreateRecurringTransaction(
            cashAccountId: account.Id, 
            transactionTypeId: transactionType.Id, 
            categoryId: category.Id);
        
        context.RecurringTransactions.Add(recurringTransaction);
        await context.SaveChangesAsync();

        // Act
        var response = await _client.GetAsync("/recurringTransactions");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var recurringTransactions = await response.Content.ReadFromJsonAsync<List<RecurringTransaction>>();
        Assert.NotNull(recurringTransactions);
        Assert.Single(recurringTransactions);
        Assert.Equal(recurringTransaction.Description, recurringTransactions[0].Description);
    }

    [Fact]
    public async Task CreateRecurringTransaction_ReturnsCreated_WhenValidRecurringTransactionProvided()
    {
        // Arrange
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<FinanceTackerDbContext>();
        await TestDataBuilder.CleanDatabase(context);

        var account = TestDataBuilder.CreateCashAccount();
        var transactionType = TestDataBuilder.CreateTransactionType();
        var category = TestDataBuilder.CreateTransactionCategory();
        
        context.CashAccounts.Add(account);
        context.TransactionTypes.Add(transactionType);
        context.TransactionCategories.Add(category);
        await context.SaveChangesAsync();

        var newRecurringTransaction = new RecurringTransaction
        {
            StartDate = DateOnly.FromDateTime(DateTime.Now),
            Amount = 100.00m,
            AmountVariancePercentage = 0.10m,
            Description = "Monthly Salary",
            RecurrenceCronExpression = "0 0 1 * *", // First day of every month
            CashAccountId = account.Id,
            TransactionTypeId = transactionType.Id,
            CategoryId = category.Id
        };

        // Act
        var response = await _client.PostAsJsonAsync("/recurringTransactions", newRecurringTransaction);

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var createdRecurringTransaction = await response.Content.ReadFromJsonAsync<RecurringTransaction>();
        Assert.NotNull(createdRecurringTransaction);
        Assert.Equal(newRecurringTransaction.Description, createdRecurringTransaction.Description);
        Assert.Equal(newRecurringTransaction.Amount, createdRecurringTransaction.Amount);
        Assert.True(createdRecurringTransaction.Id > 0);
    }

    [Fact]
    public async Task CreateRecurringTransaction_ReturnsBadRequest_WhenNullRecurringTransactionProvided()
    {
        // Act
        var response = await _client.PostAsJsonAsync("/recurringTransactions", (RecurringTransaction?)null);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task CreateRecurringTransaction_ReturnsBadRequest_WhenInvalidCashAccountIdProvided()
    {
        // Arrange
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<FinanceTackerDbContext>();
        await TestDataBuilder.CleanDatabase(context);

        var transactionType = TestDataBuilder.CreateTransactionType();
        var category = TestDataBuilder.CreateTransactionCategory();
        
        context.TransactionTypes.Add(transactionType);
        context.TransactionCategories.Add(category);
        await context.SaveChangesAsync();

        var newRecurringTransaction = new RecurringTransaction
        {
            StartDate = DateOnly.FromDateTime(DateTime.Now),
            Amount = 100.00m,
            AmountVariancePercentage = 0.10m,
            Description = "Monthly Salary",
            RecurrenceCronExpression = "0 0 1 * *",
            CashAccountId = 999, // Non-existent account
            TransactionTypeId = transactionType.Id,
            CategoryId = category.Id
        };

        // Act
        var response = await _client.PostAsJsonAsync("/recurringTransactions", newRecurringTransaction);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task CreateRecurringTransaction_CreatesWithEndDate_WhenEndDateProvided()
    {
        // Arrange
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<FinanceTackerDbContext>();
        await TestDataBuilder.CleanDatabase(context);

        var account = TestDataBuilder.CreateCashAccount();
        var transactionType = TestDataBuilder.CreateTransactionType();
        var category = TestDataBuilder.CreateTransactionCategory();
        
        context.CashAccounts.Add(account);
        context.TransactionTypes.Add(transactionType);
        context.TransactionCategories.Add(category);
        await context.SaveChangesAsync();

        var startDate = DateOnly.FromDateTime(DateTime.Now);
        var endDate = DateOnly.FromDateTime(DateTime.Now.AddMonths(12));
        
        var newRecurringTransaction = new RecurringTransaction
        {
            StartDate = startDate,
            EndDate = endDate,
            Amount = 500.00m,
            AmountVariancePercentage = 0.05m,
            Description = "Annual Subscription",
            RecurrenceCronExpression = "0 0 1 1 *", // First day of January each year
            CashAccountId = account.Id,
            TransactionTypeId = transactionType.Id,
            CategoryId = category.Id
        };

        // Act
        var response = await _client.PostAsJsonAsync("/recurringTransactions", newRecurringTransaction);

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var createdRecurringTransaction = await response.Content.ReadFromJsonAsync<RecurringTransaction>();
        Assert.NotNull(createdRecurringTransaction);
        Assert.Equal(endDate, createdRecurringTransaction.EndDate);
    }

    [Fact]
    public async Task CreateRecurringTransaction_HandlesAmountVariance_WhenVariancePercentageSet()
    {
        // Arrange
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<FinanceTackerDbContext>();
        await TestDataBuilder.CleanDatabase(context);

        var account = TestDataBuilder.CreateCashAccount();
        var transactionType = TestDataBuilder.CreateTransactionType();
        var category = TestDataBuilder.CreateTransactionCategory();
        
        context.CashAccounts.Add(account);
        context.TransactionTypes.Add(transactionType);
        context.TransactionCategories.Add(category);
        await context.SaveChangesAsync();

        var newRecurringTransaction = new RecurringTransaction
        {
            StartDate = DateOnly.FromDateTime(DateTime.Now),
            Amount = 1000.00m,
            AmountVariancePercentage = 0.20m, // 20% variance
            Description = "Variable Expense",
            RecurrenceCronExpression = "0 0 15 * *", // 15th of every month
            CashAccountId = account.Id,
            TransactionTypeId = transactionType.Id,
            CategoryId = category.Id
        };

        // Act
        var response = await _client.PostAsJsonAsync("/recurringTransactions", newRecurringTransaction);

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var createdRecurringTransaction = await response.Content.ReadFromJsonAsync<RecurringTransaction>();
        Assert.NotNull(createdRecurringTransaction);
        Assert.Equal(0.20m, createdRecurringTransaction.AmountVariancePercentage);
    }

    [Fact]
    public async Task CreateRecurringTransaction_ValidatesCronExpression_WhenCreatingRecurringTransaction()
    {
        // Arrange
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<FinanceTackerDbContext>();
        await TestDataBuilder.CleanDatabase(context);

        var account = TestDataBuilder.CreateCashAccount();
        var transactionType = TestDataBuilder.CreateTransactionType();
        var category = TestDataBuilder.CreateTransactionCategory();
        
        context.CashAccounts.Add(account);
        context.TransactionTypes.Add(transactionType);
        context.TransactionCategories.Add(category);
        await context.SaveChangesAsync();

        var newRecurringTransaction = new RecurringTransaction
        {
            StartDate = DateOnly.FromDateTime(DateTime.Now),
            Amount = 200.00m,
            AmountVariancePercentage = 0.00m,
            Description = "Weekly Payment",
            RecurrenceCronExpression = "0 0 * * 1", // Every Monday
            CashAccountId = account.Id,
            TransactionTypeId = transactionType.Id,
            CategoryId = category.Id
        };

        // Act
        var response = await _client.PostAsJsonAsync("/recurringTransactions", newRecurringTransaction);

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var createdRecurringTransaction = await response.Content.ReadFromJsonAsync<RecurringTransaction>();
        Assert.NotNull(createdRecurringTransaction);
        Assert.Equal("0 0 * * 1", createdRecurringTransaction.RecurrenceCronExpression);
    }
}
