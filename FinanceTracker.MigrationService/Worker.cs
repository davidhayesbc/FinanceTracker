using System.Diagnostics;
using FinanceTracker.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using OpenTelemetry.Trace;

namespace FinanceTracker.MigrationService;

public class Worker(
    IServiceProvider serviceProvider,
    IHostApplicationLifetime hostApplicationLifetime,
    ILogger<Worker> logger) : BackgroundService
{
    public const string ActivitySourceName = "Migrations";
    private static readonly ActivitySource ActivitySource = new(ActivitySourceName);

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        using var activity = ActivitySource.StartActivity("Migrating database", ActivityKind.Client);

        try
        {
            using var scope = serviceProvider.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<FinanceTackerDbContext>();

            await EnsureDatabaseAsync(dbContext, cancellationToken);
            await RunMigrationAsync(dbContext, cancellationToken);
            await SeedDataAsync(dbContext, cancellationToken);
        }
        catch (Exception ex)
        {
            activity?.AddException(ex);
            logger.LogError(ex, "An error occurred while migrating the database.");
            throw;
        }

        hostApplicationLifetime.StopApplication();
    }

    private static async Task EnsureDatabaseAsync(FinanceTackerDbContext dbContext, CancellationToken cancellationToken)
    {
        var dbCreator = dbContext.GetService<IRelationalDatabaseCreator>();

        var strategy = dbContext.Database.CreateExecutionStrategy();
        await strategy.ExecuteAsync(async () =>
        {
            // Create the database if it does not exist.
            // Do this first so there is then a database to start a transaction against.
            if (!await dbCreator.ExistsAsync(cancellationToken)) await dbCreator.CreateAsync(cancellationToken);
        });
    }

    private static async Task RunMigrationAsync(FinanceTackerDbContext dbContext, CancellationToken cancellationToken)
    {
        var strategy = dbContext.Database.CreateExecutionStrategy();
        await strategy.ExecuteAsync(async () =>
        {
            await dbContext.Database.MigrateAsync(cancellationToken);
        });
    }

    private static async Task SeedDataAsync(FinanceTackerDbContext dbContext, CancellationToken cancellationToken)
    {
        await SeedAccountTypesAsync(dbContext, cancellationToken);
        await SeedTransactionTypesAsync(dbContext, cancellationToken);
        await SeedCurrenciesAsync(dbContext, cancellationToken);
        await SeedSecuritiesAsync(dbContext, cancellationToken);
        await SeedCategoriesAsync(dbContext, cancellationToken);
    }

    private static async Task SeedCategoriesAsync(FinanceTackerDbContext dbContext, CancellationToken cancellationToken)
    {
        var strategy = dbContext.Database.CreateExecutionStrategy();
        await strategy.ExecuteAsync(async () =>
        {
            await using var transaction = await dbContext.Database.BeginTransactionAsync(cancellationToken);
            try
            {
                if (!await dbContext.TransactionCategories.AnyAsync(c => c.Category == "Groceries", cancellationToken))
                    await dbContext.TransactionCategories.AddAsync(new TransactionCategory { Category = "Groceries", Description = "Expenses related to grocery shopping" }, cancellationToken);

                if (!await dbContext.TransactionCategories.AnyAsync(c => c.Category == "Restaurants", cancellationToken))
                    await dbContext.TransactionCategories.AddAsync(new TransactionCategory { Category = "Restaurants", Description = "Expenses related to dining out" }, cancellationToken);

                if (!await dbContext.TransactionCategories.AnyAsync(c => c.Category == "Entertainment", cancellationToken))
                    await dbContext.TransactionCategories.AddAsync(new TransactionCategory { Category = "Entertainment", Description = "Expenses related to entertainment" }, cancellationToken);

                if (!await dbContext.TransactionCategories.AnyAsync(c => c.Category == "Utilities", cancellationToken))
                    await dbContext.TransactionCategories.AddAsync(new TransactionCategory { Category = "Utilities", Description = "Expenses related to utilities" }, cancellationToken);

                if (!await dbContext.TransactionCategories.AnyAsync(c => c.Category == "Health", cancellationToken))
                    await dbContext.TransactionCategories.AddAsync(new TransactionCategory { Category = "Health", Description = "Expenses related to health" }, cancellationToken);

                await dbContext.SaveChangesAsync(cancellationToken);
                await transaction.CommitAsync(cancellationToken);
                Console.WriteLine("SeedCategoriesAsync: Categories seeded successfully.");
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync(cancellationToken);
                Console.WriteLine($"SeedCategoriesAsync: Error seeding categories: {ex}");
                throw;
            }
        });
    }

    private static async Task SeedAccountTypesAsync(FinanceTackerDbContext dbContext, CancellationToken cancellationToken)
    {
        var strategy = dbContext.Database.CreateExecutionStrategy();
        await strategy.ExecuteAsync(async () =>
        {
            await using var transaction = await dbContext.Database.BeginTransactionAsync(cancellationToken);
            try
            {
                if (!await dbContext.AccountTypes.AnyAsync(at => at.Type == "Savings", cancellationToken))
                    await dbContext.AccountTypes.AddAsync(new AccountType { Type = "Savings" }, cancellationToken);

                if (!await dbContext.AccountTypes.AnyAsync(at => at.Type == "Current", cancellationToken))
                    await dbContext.AccountTypes.AddAsync(new AccountType { Type = "Current" }, cancellationToken);

                if (!await dbContext.AccountTypes.AnyAsync(at => at.Type == "Investment", cancellationToken))
                    await dbContext.AccountTypes.AddAsync(new AccountType { Type = "Investment" }, cancellationToken);

                if (!await dbContext.AccountTypes.AnyAsync(at => at.Type == "Property", cancellationToken))
                    await dbContext.AccountTypes.AddAsync(new AccountType { Type = "Property" }, cancellationToken);

                if (!await dbContext.AccountTypes.AnyAsync(at => at.Type == "Mortgage", cancellationToken))
                    await dbContext.AccountTypes.AddAsync(new AccountType { Type = "Mortgage" }, cancellationToken);

                if (!await dbContext.AccountTypes.AnyAsync(at => at.Type == "Credit Card", cancellationToken))
                    await dbContext.AccountTypes.AddAsync(new AccountType { Type = "Credit Card" }, cancellationToken);

                if (!await dbContext.AccountTypes.AnyAsync(at => at.Type == "Line of Credit", cancellationToken))
                    await dbContext.AccountTypes.AddAsync(new AccountType { Type = "Line of Credit" }, cancellationToken);

                await dbContext.SaveChangesAsync(cancellationToken);
                await transaction.CommitAsync(cancellationToken);
                Console.WriteLine("SeedAccountTypesAsync: Account types seeded successfully.");
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync(cancellationToken);
                Console.WriteLine($"SeedAccountTypesAsync: Error seeding account types: {ex}");
                throw;
            }
        });
    }

    private static async Task SeedTransactionTypesAsync(FinanceTackerDbContext dbContext, CancellationToken cancellationToken)
    {
        var strategy = dbContext.Database.CreateExecutionStrategy();
        await strategy.ExecuteAsync(async () =>
        {
            await using var transaction = await dbContext.Database.BeginTransactionAsync(cancellationToken);
            try
            {
                if (!await dbContext.TransactionTypes.AnyAsync(tt => tt.Type == "Buy", cancellationToken))
                    await dbContext.TransactionTypes.AddAsync(new TransactionType { Type = "Buy" }, cancellationToken);

                if (!await dbContext.TransactionTypes.AnyAsync(tt => tt.Type == "Sell", cancellationToken))
                    await dbContext.TransactionTypes.AddAsync(new TransactionType { Type = "Sell" }, cancellationToken);

                if (!await dbContext.TransactionTypes.AnyAsync(tt => tt.Type == "Short", cancellationToken))
                    await dbContext.TransactionTypes.AddAsync(new TransactionType { Type = "Short" }, cancellationToken);

                if (!await dbContext.TransactionTypes.AnyAsync(tt => tt.Type == "Cover", cancellationToken))
                    await dbContext.TransactionTypes.AddAsync(new TransactionType { Type = "Cover" }, cancellationToken);

                if (!await dbContext.TransactionTypes.AnyAsync(tt => tt.Type == "Deposit", cancellationToken))
                    await dbContext.TransactionTypes.AddAsync(new TransactionType { Type = "Deposit" }, cancellationToken);

                if (!await dbContext.TransactionTypes.AnyAsync(tt => tt.Type == "Withdrawal", cancellationToken))
                    await dbContext.TransactionTypes.AddAsync(new TransactionType { Type = "Withdrawal" }, cancellationToken);

                if (!await dbContext.TransactionTypes.AnyAsync(tt => tt.Type == "Purchase", cancellationToken))
                    await dbContext.TransactionTypes.AddAsync(new TransactionType { Type = "Purchase" }, cancellationToken);

                if (!await dbContext.TransactionTypes.AnyAsync(tt => tt.Type == "Refund", cancellationToken))
                    await dbContext.TransactionTypes.AddAsync(new TransactionType { Type = "Refund" }, cancellationToken);

                await dbContext.SaveChangesAsync(cancellationToken);
                await transaction.CommitAsync(cancellationToken);
                Console.WriteLine("SeedTransactionTypesAsync: Transaction types seeded successfully.");
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync(cancellationToken);
                Console.WriteLine($"SeedTransactionTypesAsync: Error seeding transaction types: {ex}");
                throw;
            }
        });
    }

    private static async Task SeedCurrenciesAsync(FinanceTackerDbContext dbContext, CancellationToken cancellationToken)
    {
        var strategy = dbContext.Database.CreateExecutionStrategy();
        await strategy.ExecuteAsync(async () =>
        {
            await using var transaction = await dbContext.Database.BeginTransactionAsync(cancellationToken);
            try
            {
                if (!await dbContext.Currencies.AnyAsync(c => c.Symbol == "CAD", cancellationToken))
                    await dbContext.Currencies.AddAsync(new Currency
                    {
                        Name = "Canadian Dollar",
                        Symbol = "CAD",
                        DisplaySymbol = "$"
                    }, cancellationToken);

                if (!await dbContext.Currencies.AnyAsync(c => c.Symbol == "USD", cancellationToken))
                    await dbContext.Currencies.AddAsync(new Currency
                    {
                        Name = "US Dollar",
                        Symbol = "USD",
                        DisplaySymbol = "$"
                    }, cancellationToken);

                await dbContext.SaveChangesAsync(cancellationToken);
                await transaction.CommitAsync(cancellationToken);
                Console.WriteLine("SeedCurrenciesAsync: Currencies seeded successfully.");
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync(cancellationToken);
                Console.WriteLine($"SeedCurrenciesAsync: Error seeding currencies: {ex}");
                throw;
            }
        });
    }

    private static async Task SeedSecuritiesAsync(FinanceTackerDbContext dbContext, CancellationToken cancellationToken)
    {
        var strategy = dbContext.Database.CreateExecutionStrategy();
        await strategy.ExecuteAsync(async () =>
        {
            await using var transaction = await dbContext.Database.BeginTransactionAsync(cancellationToken);
            try
            {
                if (!await dbContext.Securities.AnyAsync(s => s.Symbol == "CAD-CASH", cancellationToken))
                    await dbContext.Securities.AddAsync(new Security
                    {
                        Name = "Canadian Cash",
                        Symbol = "CAD-CASH",
                        CurrencyId = dbContext.Currencies.First(c => c.Symbol == "CAD").Id,
                        SecurityType = "Cash"
                    }, cancellationToken);

                if (!await dbContext.Securities.AnyAsync(s => s.Symbol == "USD-CASH", cancellationToken))
                    await dbContext.Securities.AddAsync(new Security
                    {
                        Name = "US Cash",
                        Symbol = "USD-CASH",
                        CurrencyId = dbContext.Currencies.First(c => c.Symbol == "USD").Id,
                        SecurityType = "Cash"
                    }, cancellationToken);

                await dbContext.SaveChangesAsync(cancellationToken);
                await transaction.CommitAsync(cancellationToken);
                Console.WriteLine("SeedSecuritiesAsync: Securities seeded successfully.");
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync(cancellationToken);
                Console.WriteLine($"SeedSecuritiesAsync: Error seeding securities: {ex}");
                throw;
            }
        });
    }
}
