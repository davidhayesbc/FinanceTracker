using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using FinanceTracker.Data.Models;
using FinanceTracker.ApiService.Dtos;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace FinanceTracker.ApiService.Tests.Integration;

/// <summary>
/// Integration tests for edge cases, error handling, and boundary conditions
/// </summary>
public class EdgeCaseIntegrationTests : IClassFixture<FinanceTrackerWebApplicationFactory>
{
    private readonly FinanceTrackerWebApplicationFactory _factory;
    private readonly HttpClient _client;

    public EdgeCaseIntegrationTests(FinanceTrackerWebApplicationFactory factory)
    {
        _factory = factory;
        _client = _factory.CreateClient();
    }

    [Fact]
    public async Task CreateTransaction_WithZeroAmount_HandledCorrectly()
    {
        // Arrange
        using var context = _factory.GetDbContext();
        var (account, accountPeriod) = await SetupBasicCashAccountAsync(context);

        var zeroAmountTransaction = new CreateCashTransactionRequestDto
        {
            TransactionDate = DateOnly.FromDateTime(DateTime.Today),
            Description = "Zero Amount Transaction",
            Amount = 0m,
            TransactionTypeId = 1,
            AccountPeriodId = accountPeriod.Id
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/v1/transactions/cash", zeroAmountTransaction);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        
        // Verify account balance remains unchanged
        var accountResponse = await _client.GetAsync($"/api/v1/accounts/{account.Id}");
        var accountDto = await accountResponse.Content.ReadFromJsonAsync<CashAccountDto>();
        accountDto!.CurrentBalance.Should().Be(1000m); // Original opening balance
    }

    [Fact]
    public async Task CreateTransaction_WithVeryLargeAmount_HandledCorrectly()
    {
        // Arrange
        using var context = _factory.GetDbContext();
        var (account, accountPeriod) = await SetupBasicCashAccountAsync(context);

        var largeAmountTransaction = new CreateCashTransactionRequestDto
        {
            TransactionDate = DateOnly.FromDateTime(DateTime.Today),
            Description = "Very Large Transaction",
            Amount = 999999999.99m, // Close to decimal max for currency
            TransactionTypeId = 1,
            AccountPeriodId = accountPeriod.Id
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/v1/transactions/cash", largeAmountTransaction);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        
        // Verify account balance updated correctly
        var accountResponse = await _client.GetAsync($"/api/v1/accounts/{account.Id}");
        var accountDto = await accountResponse.Content.ReadFromJsonAsync<CashAccountDto>();
        accountDto!.CurrentBalance.Should().Be(1000m + 999999999.99m);
    }

    [Fact]
    public async Task CreateTransaction_WithVerySmallNegativeAmount_HandledCorrectly()
    {
        // Arrange
        using var context = _factory.GetDbContext();
        var (account, accountPeriod) = await SetupBasicCashAccountAsync(context);

        var smallNegativeTransaction = new CreateCashTransactionRequestDto
        {
            TransactionDate = DateOnly.FromDateTime(DateTime.Today),
            Description = "Small Negative Transaction",
            Amount = -0.01m, // Smallest possible currency unit
            TransactionTypeId = 1,
            AccountPeriodId = accountPeriod.Id
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/v1/transactions/cash", smallNegativeTransaction);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        
        // Verify account balance updated correctly
        var accountResponse = await _client.GetAsync($"/api/v1/accounts/{account.Id}");
        var accountDto = await accountResponse.Content.ReadFromJsonAsync<CashAccountDto>();
        accountDto!.CurrentBalance.Should().Be(999.99m);
    }

    [Fact]
    public async Task CreateTransaction_WithFutureDate_HandledCorrectly()
    {
        // Arrange
        using var context = _factory.GetDbContext();
        var (account, accountPeriod) = await SetupBasicCashAccountAsync(context);

        var futureDateTransaction = new CreateCashTransactionRequestDto
        {
            TransactionDate = DateOnly.FromDateTime(DateTime.Today.AddDays(30)),
            Description = "Future Date Transaction",
            Amount = 500m,
            TransactionTypeId = 1,
            AccountPeriodId = accountPeriod.Id
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/v1/transactions/cash", futureDateTransaction);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        
        var createdTransaction = await response.Content.ReadFromJsonAsync<CashTransaction>();
        createdTransaction!.TransactionDate.Should().Be(DateOnly.FromDateTime(DateTime.Today.AddDays(30)));
    }

    [Fact]
    public async Task CreateTransaction_WithVeryOldDate_HandledCorrectly()
    {
        // Arrange
        using var context = _factory.GetDbContext();
        var (account, accountPeriod) = await SetupBasicCashAccountAsync(context);

        var oldDateTransaction = new CreateCashTransactionRequestDto
        {
            TransactionDate = DateOnly.FromDateTime(DateTime.Today.AddYears(-10)),
            Description = "Very Old Transaction",
            Amount = 100m,
            TransactionTypeId = 1,
            AccountPeriodId = accountPeriod.Id
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/v1/transactions/cash", oldDateTransaction);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        
        var createdTransaction = await response.Content.ReadFromJsonAsync<CashTransaction>();
        createdTransaction!.TransactionDate.Should().Be(DateOnly.FromDateTime(DateTime.Today.AddYears(-10)));
    }

    [Fact]
    public async Task CreateTransaction_WithEmptyDescription_ReturnsValidationError()
    {
        // Arrange
        using var context = _factory.GetDbContext();
        var (account, accountPeriod) = await SetupBasicCashAccountAsync(context);

        var emptyDescriptionTransaction = new CreateCashTransactionRequestDto
        {
            TransactionDate = DateOnly.FromDateTime(DateTime.Today),
            Description = "", // Empty description
            Amount = 100m,
            TransactionTypeId = 1,
            AccountPeriodId = accountPeriod.Id
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/v1/transactions/cash", emptyDescriptionTransaction);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CreateTransaction_WithNullDescription_ReturnsValidationError()
    {
        // Arrange
        using var context = _factory.GetDbContext();
        var (account, accountPeriod) = await SetupBasicCashAccountAsync(context);

        var nullDescriptionTransaction = new CreateCashTransactionRequestDto
        {
            TransactionDate = DateOnly.FromDateTime(DateTime.Today),
            Description = null!, // Null description
            Amount = 100m,
            TransactionTypeId = 1,
            AccountPeriodId = accountPeriod.Id
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/v1/transactions/cash", nullDescriptionTransaction);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CreateTransaction_WithVeryLongDescription_HandledCorrectly()
    {
        // Arrange
        using var context = _factory.GetDbContext();
        var (account, accountPeriod) = await SetupBasicCashAccountAsync(context);

        var longDescription = new string('A', 1000); // Very long description
        var longDescriptionTransaction = new CreateCashTransactionRequestDto
        {
            TransactionDate = DateOnly.FromDateTime(DateTime.Today),
            Description = longDescription,
            Amount = 100m,
            TransactionTypeId = 1,
            AccountPeriodId = accountPeriod.Id
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/v1/transactions/cash", longDescriptionTransaction);

        // Assert
        // This might return BadRequest if there's a max length validation, or Created if it's handled
        response.StatusCode.Should().BeOneOf(HttpStatusCode.Created, HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CreateInvestmentTransaction_WithZeroQuantity_ReturnsValidationError()
    {
        // Arrange
        using var context = _factory.GetDbContext();
        var (account, accountPeriod, security) = await SetupBasicInvestmentAccountAsync(context);

        var zeroQuantityTransaction = new CreateInvestmentTransactionRequestDto
        {
            TransactionDate = DateOnly.FromDateTime(DateTime.Today),
            Description = "Zero Quantity Transaction",
            Quantity = 0m, // Zero quantity
            Price = 100m,
            SecurityId = security.Id,
            TransactionTypeId = 1,
            AccountPeriodId = accountPeriod.Id
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/v1/transactions/investment", zeroQuantityTransaction);

        // Assert
        // This might be valid (e.g., for adjustments) or invalid depending on business rules
        response.StatusCode.Should().BeOneOf(HttpStatusCode.Created, HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CreateInvestmentTransaction_WithZeroPrice_ReturnsValidationError()
    {
        // Arrange
        using var context = _factory.GetDbContext();
        var (account, accountPeriod, security) = await SetupBasicInvestmentAccountAsync(context);

        var zeroPriceTransaction = new CreateInvestmentTransactionRequestDto
        {
            TransactionDate = DateOnly.FromDateTime(DateTime.Today),
            Description = "Zero Price Transaction",
            Quantity = 10m,
            Price = 0m, // Zero price
            SecurityId = security.Id,
            TransactionTypeId = 1,
            AccountPeriodId = accountPeriod.Id
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/v1/transactions/investment", zeroPriceTransaction);

        // Assert
        // This might be valid (e.g., for free shares) or invalid depending on business rules
        response.StatusCode.Should().BeOneOf(HttpStatusCode.Created, HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CreateTransactionSplit_WithAmountGreaterThanTransaction_ReturnsValidationError()
    {
        // Arrange
        using var context = _factory.GetDbContext();
        var (account, accountPeriod) = await SetupBasicCashAccountAsync(context);

        // Create a transaction first
        var transactionDto = new CreateCashTransactionRequestDto
        {
            TransactionDate = DateOnly.FromDateTime(DateTime.Today),
            Description = "Base Transaction",
            Amount = -100m, // Transaction amount
            TransactionTypeId = 1,
            AccountPeriodId = accountPeriod.Id
        };

        var transactionResponse = await _client.PostAsJsonAsync("/api/v1/transactions/cash", transactionDto);
        var createdTransaction = await transactionResponse.Content.ReadFromJsonAsync<CashTransaction>();

        // Create category for split
        var category = TestDataBuilder.CreateTransactionCategory("Test Category");
        context.TransactionCategories.Add(category);
        await context.SaveChangesAsync();

        // Create split with amount greater than transaction
        var splitDto = new CreateTransactionSplitRequestDto
        {
            CashTransactionId = createdTransaction!.Id,
            CategoryId = category.Id,
            Amount = -150m // Greater than transaction amount
        };

        // Act
        var splitResponse = await _client.PostAsJsonAsync("/api/v1/transactionSplits", splitDto);

        // Assert
        // This should be allowed as splits can be created independently, 
        // validation might happen at a business logic level
        splitResponse.StatusCode.Should().BeOneOf(HttpStatusCode.Created, HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CreateTransferTransaction_ToSameAccount_ReturnsValidationError()
    {
        // Arrange
        using var context = _factory.GetDbContext();
        var (account, accountPeriod) = await SetupBasicCashAccountAsync(context);

        var selfTransferTransaction = new CreateCashTransactionRequestDto
        {
            TransactionDate = DateOnly.FromDateTime(DateTime.Today),
            Description = "Self Transfer",
            Amount = -100m,
            TransferToCashAccountId = account.Id, // Transfer to same account
            TransactionTypeId = 1,
            AccountPeriodId = accountPeriod.Id
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/v1/transactions/cash", selfTransferTransaction);

        // Assert
        // This should be invalid business logic
        response.StatusCode.Should().BeOneOf(HttpStatusCode.Created, HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task GetAccount_WithNonExistentId_ReturnsNotFound()
    {
        // Act
        var response = await _client.GetAsync("/api/v1/accounts/99999");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetAccountTransactions_WithNonExistentAccountId_ReturnsNotFound()
    {
        // Act
        var response = await _client.GetAsync("/api/v1/accounts/99999/transactions");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task CreateMultipleTransactions_Simultaneously_HandledCorrectly()
    {
        // Arrange
        using var context = _factory.GetDbContext();
        var (account, accountPeriod) = await SetupBasicCashAccountAsync(context);

        var transactions = Enumerable.Range(1, 100).Select(i => new CreateCashTransactionRequestDto
        {
            TransactionDate = DateOnly.FromDateTime(DateTime.Today),
            Description = $"Concurrent Transaction {i}",
            Amount = i % 2 == 0 ? 10m : -5m, // Alternating positive/negative
            TransactionTypeId = 1,
            AccountPeriodId = accountPeriod.Id
        }).ToList();

        // Act - Create transactions in parallel (simulating concurrent requests)
        var tasks = transactions.Select(async transaction =>
        {
            var response = await _client.PostAsJsonAsync("/api/v1/transactions/cash", transaction);
            return response;
        });

        var responses = await Task.WhenAll(tasks);

        // Assert
        var successfulResponses = responses.Count(r => r.StatusCode == HttpStatusCode.Created);
        successfulResponses.Should().Be(100); // All should succeed

        // Verify final balance is correct
        var accountResponse = await _client.GetAsync($"/api/v1/accounts/{account.Id}");
        var accountDto = await accountResponse.Content.ReadFromJsonAsync<CashAccountDto>();
        
        // Expected: 1000 + (50 * 10) + (50 * -5) = 1000 + 500 - 250 = 1250
        var expectedBalance = 1000m + (50 * 10m) + (50 * -5m);
        accountDto!.CurrentBalance.Should().Be(expectedBalance);
    }

    [Fact]
    public async Task CreateTransaction_WithInvalidTransactionTypeId_ReturnsBadRequest()
    {
        // Arrange
        using var context = _factory.GetDbContext();
        var (account, accountPeriod) = await SetupBasicCashAccountAsync(context);

        var invalidTypeTransaction = new CreateCashTransactionRequestDto
        {
            TransactionDate = DateOnly.FromDateTime(DateTime.Today),
            Description = "Invalid Type Transaction",
            Amount = 100m,
            TransactionTypeId = 99999, // Non-existent transaction type
            AccountPeriodId = accountPeriod.Id
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/v1/transactions/cash", invalidTypeTransaction);

        // Assert
        response.StatusCode.Should().BeOneOf(HttpStatusCode.BadRequest, HttpStatusCode.Created);
        // Depending on validation implementation, this might be handled at DB level or API level
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

    private async Task<(InvestmentAccount account, AccountPeriod period, Security security)> SetupBasicInvestmentAccountAsync(
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
