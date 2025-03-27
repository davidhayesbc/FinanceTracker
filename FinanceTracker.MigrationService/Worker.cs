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
        var strategy = dbContext.Database.CreateExecutionStrategy();
        await strategy.ExecuteAsync(async () =>
        {
            // Seed the database
            await using var transaction = await dbContext.Database.BeginTransactionAsync(cancellationToken);

            // Account Types
            if (!await dbContext.AccountTypes.AnyAsync(at => at.Type == "Savings", cancellationToken))
                await dbContext.AccountTypes.AddAsync(new AccountType
                {
                    Type = "Savings"
                }, cancellationToken);

            if (!await dbContext.AccountTypes.AnyAsync(at => at.Type == "Current", cancellationToken))
                await dbContext.AccountTypes.AddAsync(new AccountType
                {
                    Type = "Current"
                }, cancellationToken);

            if (!await dbContext.AccountTypes.AnyAsync(at => at.Type == "Investment", cancellationToken))
                await dbContext.AccountTypes.AddAsync(new AccountType
                {
                    Type = "Investment"
                }, cancellationToken);

            if (!await dbContext.AccountTypes.AnyAsync(at => at.Type == "Property", cancellationToken))
                await dbContext.AccountTypes.AddAsync(new AccountType
                {
                    Type = "Property"
                }, cancellationToken);

            if (!await dbContext.AccountTypes.AnyAsync(at => at.Type == "Mortgage", cancellationToken))
                await dbContext.AccountTypes.AddAsync(new AccountType
                {
                    Type = "Mortgage"
                }, cancellationToken);

            // Transaction Types
            if (!await dbContext.TransactionTypes.AnyAsync(tt => tt.Type == "Change", cancellationToken))
                await dbContext.TransactionTypes.AddAsync(new TransactionType
                {
                    Type = "Change"
                }, cancellationToken);

            if (!await dbContext.TransactionTypes.AnyAsync(tt => tt.Type == "BalanceUpdate", cancellationToken))
                await dbContext.TransactionTypes.AddAsync(new TransactionType
                {
                    Type = "BalanceUpdate"
                }, cancellationToken);
            // Currencies
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
            // Securities
            if (!await dbContext.Securities.AnyAsync(s => s.Symbol == "CAD-CASH", cancellationToken))
                await dbContext.Securities.AddAsync(new Security
                {
                    Name = "Canadian Cash",
                    Symbol = "CAD",
                    CurrencyId = dbContext.Currencies.First(c => c.Symbol == "CAD").Id,
                    SecurityType = "Cash"
                }, cancellationToken);

            if (!await dbContext.Securities.AnyAsync(s => s.Symbol == "USD-CASH", cancellationToken))
                await dbContext.Securities.AddAsync(new Security
                {
                    Name = "US Cash",
                    Symbol = "USD",
                    CurrencyId = dbContext.Currencies.First(c => c.Symbol == "USD").Id,
                    SecurityType = "Cash"
                }, cancellationToken);
            await dbContext.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
        });
    }
}
