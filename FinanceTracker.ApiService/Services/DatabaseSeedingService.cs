using FinanceTracker.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace FinanceTracker.ApiService.Services;

/// <summary>
/// Service responsible for seeding the database with reference data when using in-memory database
/// This ensures that tests have the necessary reference data available
/// </summary>
public class DatabaseSeedingService
{
    private readonly FinanceTackerDbContext _context;
    private readonly ILogger<DatabaseSeedingService> _logger;

    public DatabaseSeedingService(FinanceTackerDbContext context, ILogger<DatabaseSeedingService> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <summary>
    /// Seeds the database with reference data required for the application to function
    /// This method is idempotent - it can be called multiple times without causing issues
    /// </summary>
    public async Task SeedAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Starting database seeding...");

            await SeedAccountTypesAsync(cancellationToken);
            await SeedTransactionTypesAsync(cancellationToken);
            await SeedCurrenciesAsync(cancellationToken);
            await SeedSecuritiesAsync(cancellationToken);
            await SeedCategoriesAsync(cancellationToken);

            _logger.LogInformation("Database seeding completed successfully.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while seeding the database.");
            throw;
        }
    }

    private async Task SeedAccountTypesAsync(CancellationToken cancellationToken)
    {
        if (!await _context.AccountTypes.AnyAsync(at => at.Type == "Savings", cancellationToken))
            await _context.AccountTypes.AddAsync(new AccountType { Type = "Savings" }, cancellationToken);

        if (!await _context.AccountTypes.AnyAsync(at => at.Type == "Current", cancellationToken))
            await _context.AccountTypes.AddAsync(new AccountType { Type = "Current" }, cancellationToken);

        if (!await _context.AccountTypes.AnyAsync(at => at.Type == "Investment", cancellationToken))
            await _context.AccountTypes.AddAsync(new AccountType { Type = "Investment" }, cancellationToken);

        if (!await _context.AccountTypes.AnyAsync(at => at.Type == "Property", cancellationToken))
            await _context.AccountTypes.AddAsync(new AccountType { Type = "Property" }, cancellationToken);

        if (!await _context.AccountTypes.AnyAsync(at => at.Type == "Mortgage", cancellationToken))
            await _context.AccountTypes.AddAsync(new AccountType { Type = "Mortgage" }, cancellationToken);

        if (!await _context.AccountTypes.AnyAsync(at => at.Type == "Credit Card", cancellationToken))
            await _context.AccountTypes.AddAsync(new AccountType { Type = "Credit Card" }, cancellationToken);

        if (!await _context.AccountTypes.AnyAsync(at => at.Type == "Line of Credit", cancellationToken))
            await _context.AccountTypes.AddAsync(new AccountType { Type = "Line of Credit" }, cancellationToken);

        await _context.SaveChangesAsync(cancellationToken);
        _logger.LogDebug("Account types seeded successfully.");
    }

    private async Task SeedTransactionTypesAsync(CancellationToken cancellationToken)
    {
        if (!await _context.TransactionTypes.AnyAsync(tt => tt.Type == "Buy", cancellationToken))
            await _context.TransactionTypes.AddAsync(new TransactionType { Type = "Buy" }, cancellationToken);

        if (!await _context.TransactionTypes.AnyAsync(tt => tt.Type == "Sell", cancellationToken))
            await _context.TransactionTypes.AddAsync(new TransactionType { Type = "Sell" }, cancellationToken);

        if (!await _context.TransactionTypes.AnyAsync(tt => tt.Type == "Short", cancellationToken))
            await _context.TransactionTypes.AddAsync(new TransactionType { Type = "Short" }, cancellationToken);

        if (!await _context.TransactionTypes.AnyAsync(tt => tt.Type == "Cover", cancellationToken))
            await _context.TransactionTypes.AddAsync(new TransactionType { Type = "Cover" }, cancellationToken);

        if (!await _context.TransactionTypes.AnyAsync(tt => tt.Type == "Deposit", cancellationToken))
            await _context.TransactionTypes.AddAsync(new TransactionType { Type = "Deposit" }, cancellationToken);

        if (!await _context.TransactionTypes.AnyAsync(tt => tt.Type == "Withdrawal", cancellationToken))
            await _context.TransactionTypes.AddAsync(new TransactionType { Type = "Withdrawal" }, cancellationToken);

        if (!await _context.TransactionTypes.AnyAsync(tt => tt.Type == "Purchase", cancellationToken))
            await _context.TransactionTypes.AddAsync(new TransactionType { Type = "Purchase" }, cancellationToken);

        if (!await _context.TransactionTypes.AnyAsync(tt => tt.Type == "Refund", cancellationToken))
            await _context.TransactionTypes.AddAsync(new TransactionType { Type = "Refund" }, cancellationToken);

        await _context.SaveChangesAsync(cancellationToken);
        _logger.LogDebug("Transaction types seeded successfully.");
    }

    private async Task SeedCurrenciesAsync(CancellationToken cancellationToken)
    {
        if (!await _context.Currencies.AnyAsync(c => c.Symbol == "CAD", cancellationToken))
            await _context.Currencies.AddAsync(new Currency
            {
                Name = "Canadian Dollar",
                Symbol = "CAD",
                DisplaySymbol = "$"
            }, cancellationToken);

        if (!await _context.Currencies.AnyAsync(c => c.Symbol == "USD", cancellationToken))
            await _context.Currencies.AddAsync(new Currency
            {
                Name = "US Dollar",
                Symbol = "USD",
                DisplaySymbol = "$"
            }, cancellationToken);

        await _context.SaveChangesAsync(cancellationToken);
        _logger.LogDebug("Currencies seeded successfully.");
    }

    private async Task SeedSecuritiesAsync(CancellationToken cancellationToken)
    {
        if (!await _context.Securities.AnyAsync(s => s.Symbol == "CAD-CASH", cancellationToken))
            await _context.Securities.AddAsync(new Security
            {
                Name = "Canadian Cash",
                Symbol = "CAD-CASH",
                SecurityType = "Cash"
            }, cancellationToken);

        if (!await _context.Securities.AnyAsync(s => s.Symbol == "USD-CASH", cancellationToken))
            await _context.Securities.AddAsync(new Security
            {
                Name = "US Cash",
                Symbol = "USD-CASH",
                SecurityType = "Cash"
            }, cancellationToken);

        await _context.SaveChangesAsync(cancellationToken);
        _logger.LogDebug("Securities seeded successfully.");
    }

    private async Task SeedCategoriesAsync(CancellationToken cancellationToken)
    {
        if (!await _context.TransactionCategories.AnyAsync(tc => tc.Category == "Groceries", cancellationToken))
            await _context.TransactionCategories.AddAsync(new TransactionCategory
            {
                Category = "Groceries",
                Description = "Expenses related to grocery shopping"
            }, cancellationToken);

        if (!await _context.TransactionCategories.AnyAsync(tc => tc.Category == "Restaurants", cancellationToken))
            await _context.TransactionCategories.AddAsync(new TransactionCategory
            {
                Category = "Restaurants",
                Description = "Expenses related to dining out"
            }, cancellationToken);

        if (!await _context.TransactionCategories.AnyAsync(tc => tc.Category == "Entertainment", cancellationToken))
            await _context.TransactionCategories.AddAsync(new TransactionCategory
            {
                Category = "Entertainment",
                Description = "Expenses related to entertainment"
            }, cancellationToken);

        if (!await _context.TransactionCategories.AnyAsync(tc => tc.Category == "Utilities", cancellationToken))
            await _context.TransactionCategories.AddAsync(new TransactionCategory
            {
                Category = "Utilities",
                Description = "Expenses related to utilities"
            }, cancellationToken);

        if (!await _context.TransactionCategories.AnyAsync(tc => tc.Category == "Health", cancellationToken))
            await _context.TransactionCategories.AddAsync(new TransactionCategory
            {
                Category = "Health",
                Description = "Expenses related to health"
            }, cancellationToken);

        await _context.SaveChangesAsync(cancellationToken);
        _logger.LogDebug("Transaction categories seeded successfully.");
    }
}
