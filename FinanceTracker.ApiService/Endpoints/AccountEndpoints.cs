using FinanceTracker.Data.Models;
using FinanceTracker.ApiService.Dtos;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using System;

namespace FinanceTracker.ApiService.Endpoints;

public static class AccountEndpoints
{
    public static void MapAccountEndpoints(this IEndpointRouteBuilder group)
    {
        var accountEndpoints = group.MapGroup("/accounts")
            .WithTags("Accounts")
            .WithGroupName("Accounts");

        // Get all accounts (both cash and investment)
        accountEndpoints.MapGet("/", async (FinanceTackerDbContext context) =>
        {
            var cashAccounts = await context.CashAccounts
                .Include(a => a.AccountType)
                .Include(a => a.Currency)
                .Include(a => a.AccountPeriods)
                .Select(a => new CashAccountDto
                {
                    Id = a.Id,
                    Name = a.Name,
                    Institution = a.Institution,
                    AccountTypeName = a.AccountType != null ? a.AccountType.Type : string.Empty,
                    CurrencySymbol = a.Currency != null ? a.Currency.Symbol : string.Empty,
                    CurrencyDisplaySymbol = a.Currency != null ? a.Currency.DisplaySymbol : string.Empty,
                    IsActive = a.IsActive,
                    AccountKind = "Cash",
                    OverdraftLimit = a.OverdraftLimit,
                    CurrentBalance = a.AccountPeriods
                        .Where(ap => ap.PeriodCloseDate == null)
                        .Select(ap => ap.OpeningBalance + ap.Transactions.OfType<CashTransaction>().Sum(t => t.Amount))
                        .FirstOrDefault()
                })
                .ToListAsync();

            var investmentAccounts = await context.InvestmentAccounts
                .Include(a => a.AccountType)
                .Include(a => a.Currency)
                .Include(a => a.AccountPeriods)
                .Select(a => new InvestmentAccountDto
                {
                    Id = a.Id,
                    Name = a.Name,
                    Institution = a.Institution,
                    AccountTypeName = a.AccountType != null ? a.AccountType.Type : string.Empty,
                    CurrencySymbol = a.Currency != null ? a.Currency.Symbol : string.Empty,
                    CurrencyDisplaySymbol = a.Currency != null ? a.Currency.DisplaySymbol : string.Empty,
                    IsActive = a.IsActive,
                    AccountKind = "Investment",
                    BrokerAccountNumber = a.BrokerAccountNumber,
                    IsTaxAdvantaged = a.IsTaxAdvantaged,
                    TaxAdvantageType = a.TaxAdvantageType,
                    CurrentBalance = a.AccountPeriods
                        .Where(ap => ap.PeriodCloseDate == null)
                        .Select(ap => ap.OpeningBalance +
                            ap.Transactions.OfType<InvestmentTransaction>().Sum(t => t.Quantity * t.Price))
                        .FirstOrDefault()
                })
                .ToListAsync();

            var allAccounts = new List<AccountBaseDto>();
            allAccounts.AddRange(cashAccounts);
            allAccounts.AddRange(investmentAccounts);

            return Results.Ok(allAccounts);
        })
        .WithName("GetAllAccounts")
        .WithDescription("Gets all accounts (both cash and investment) with calculated balances.")
        .Produces<List<AccountBaseDto>>(StatusCodes.Status200OK);

        // Get cash accounts only
        accountEndpoints.MapGet("/cash", async (FinanceTackerDbContext context) =>
        {
            var cashAccounts = await context.CashAccounts
                .Include(a => a.AccountType)
                .Include(a => a.Currency)
                .Include(a => a.AccountPeriods)
                .Select(a => new CashAccountDto
                {
                    Id = a.Id,
                    Name = a.Name,
                    Institution = a.Institution,
                    AccountTypeName = a.AccountType != null ? a.AccountType.Type : string.Empty,
                    CurrencySymbol = a.Currency != null ? a.Currency.Symbol : string.Empty,
                    CurrencyDisplaySymbol = a.Currency != null ? a.Currency.DisplaySymbol : string.Empty,
                    IsActive = a.IsActive,
                    AccountKind = "Cash",
                    OverdraftLimit = a.OverdraftLimit,
                    CurrentBalance = a.AccountPeriods
                        .Where(ap => ap.PeriodCloseDate == null)
                        .Select(ap => ap.OpeningBalance + ap.Transactions.OfType<CashTransaction>().Sum(t => t.Amount))
                        .FirstOrDefault()
                })
                .ToListAsync();

            return Results.Ok(cashAccounts);
        })
        .WithName("GetCashAccounts")
        .WithDescription("Gets all cash accounts with calculated balances.")
        .Produces<List<CashAccountDto>>(StatusCodes.Status200OK);

        // Get investment accounts only
        accountEndpoints.MapGet("/investment", async (FinanceTackerDbContext context) =>
        {
            var investmentAccounts = await context.InvestmentAccounts
                .Include(a => a.AccountType)
                .Include(a => a.Currency)
                .Include(a => a.AccountPeriods)
                .Select(a => new InvestmentAccountDto
                {
                    Id = a.Id,
                    Name = a.Name,
                    Institution = a.Institution,
                    AccountTypeName = a.AccountType != null ? a.AccountType.Type : string.Empty,
                    CurrencySymbol = a.Currency != null ? a.Currency.Symbol : string.Empty,
                    CurrencyDisplaySymbol = a.Currency != null ? a.Currency.DisplaySymbol : string.Empty,
                    IsActive = a.IsActive,
                    AccountKind = "Investment",
                    BrokerAccountNumber = a.BrokerAccountNumber,
                    IsTaxAdvantaged = a.IsTaxAdvantaged,
                    TaxAdvantageType = a.TaxAdvantageType,
                    CurrentBalance = a.AccountPeriods
                        .Where(ap => ap.PeriodCloseDate == null)
                        .Select(ap => ap.OpeningBalance +
                            ap.Transactions.OfType<InvestmentTransaction>().Sum(t => t.Quantity * t.Price))
                        .FirstOrDefault()
                })
                .ToListAsync();

            return Results.Ok(investmentAccounts);
        })
        .WithName("GetInvestmentAccounts")
        .WithDescription("Gets all investment accounts with calculated balances.")
        .Produces<List<InvestmentAccountDto>>(StatusCodes.Status200OK);

        // Get account by ID (determines type automatically)
        accountEndpoints.MapGet("/{id}", async (FinanceTackerDbContext context, int id) =>
        {
            // Try to find as cash account first
            var cashAccount = await context.CashAccounts
                .Where(a => a.Id == id)
                .Include(a => a.AccountType)
                .Include(a => a.Currency)
                .Include(a => a.AccountPeriods)
                .Select(a => new CashAccountDto
                {
                    Id = a.Id,
                    Name = a.Name,
                    Institution = a.Institution,
                    AccountTypeName = a.AccountType != null ? a.AccountType.Type : string.Empty,
                    CurrencySymbol = a.Currency != null ? a.Currency.Symbol : string.Empty,
                    CurrencyDisplaySymbol = a.Currency != null ? a.Currency.DisplaySymbol : string.Empty,
                    IsActive = a.IsActive,
                    AccountKind = "Cash",
                    OverdraftLimit = a.OverdraftLimit,
                    CurrentBalance = a.AccountPeriods
                        .Where(ap => ap.PeriodCloseDate == null)
                        .Select(ap => ap.OpeningBalance + ap.Transactions.OfType<CashTransaction>().Sum(t => t.Amount))
                        .FirstOrDefault()
                })
                .FirstOrDefaultAsync();

            if (cashAccount != null)
            {
                return Results.Ok(cashAccount);
            }

            // Try to find as investment account
            var investmentAccount = await context.InvestmentAccounts
                .Where(a => a.Id == id)
                .Include(a => a.AccountType)
                .Include(a => a.Currency)
                .Include(a => a.AccountPeriods)
                .Select(a => new InvestmentAccountDto
                {
                    Id = a.Id,
                    Name = a.Name,
                    Institution = a.Institution,
                    AccountTypeName = a.AccountType != null ? a.AccountType.Type : string.Empty,
                    CurrencySymbol = a.Currency != null ? a.Currency.Symbol : string.Empty,
                    CurrencyDisplaySymbol = a.Currency != null ? a.Currency.DisplaySymbol : string.Empty,
                    IsActive = a.IsActive,
                    AccountKind = "Investment",
                    BrokerAccountNumber = a.BrokerAccountNumber,
                    IsTaxAdvantaged = a.IsTaxAdvantaged,
                    TaxAdvantageType = a.TaxAdvantageType,
                    CurrentBalance = a.AccountPeriods
                        .Where(ap => ap.PeriodCloseDate == null)
                        .Select(ap => ap.OpeningBalance +
                            ap.Transactions.OfType<InvestmentTransaction>().Sum(t => t.Quantity * t.Price))
                        .FirstOrDefault()
                })
                .FirstOrDefaultAsync();

            if (investmentAccount != null)
            {
                return Results.Ok(investmentAccount);
            }

            return Results.NotFound($"Account with ID {id} not found.");
        })
        .WithName("GetAccountById")
        .WithDescription("Gets an account by its ID, including calculated current balance.")
        .Produces<AccountBaseDto>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status404NotFound);

        // Get account transactions
        accountEndpoints.MapGet("/{id}/transactions", async (FinanceTackerDbContext context, int id) =>
        {
            // Check if it's a cash account
            var cashAccountExists = await context.CashAccounts.AnyAsync(a => a.Id == id);
            if (cashAccountExists)
            {
                var cashTransactions = await context.CashTransactions
                    .Include(t => t.AccountPeriod)
                    .Include(t => t.TransactionType)
                    .Include(t => t.Category)
                    .Include(t => t.TransferToCashAccount)
                    .Where(t => t.AccountPeriod.AccountId == id && t.AccountPeriod.PeriodCloseDate == null)
                    .Select(t => new CashTransactionDto
                    {
                        Id = t.Id,
                        AccountId = t.AccountPeriod.AccountId,
                        TransactionDate = t.TransactionDate,
                        Description = t.Description,
                        TransactionTypeName = t.TransactionType.Type,
                        CategoryName = t.Category.Category,
                        TransactionKind = "Cash",
                        Amount = t.Amount,
                        TransferToCashAccountId = t.TransferToCashAccountId,
                        TransferToCashAccountName = t.TransferToCashAccount != null ? t.TransferToCashAccount.Name : null
                    })
                    .ToListAsync();

                return Results.Ok(cashTransactions);
            }

            // Check if it's an investment account
            var investmentAccountExists = await context.InvestmentAccounts.AnyAsync(a => a.Id == id);
            if (investmentAccountExists)
            {
                var investmentTransactions = await context.InvestmentTransactions
                    .Include(t => t.AccountPeriod)
                    .Include(t => t.TransactionType)
                    .Include(t => t.Category)
                    .Include(t => t.Security)
                        .ThenInclude(s => s.Currency)
                    .Where(t => t.AccountPeriod.AccountId == id && t.AccountPeriod.PeriodCloseDate == null)
                    .Select(t => new InvestmentTransactionDto
                    {
                        Id = t.Id,
                        AccountId = t.AccountPeriod.AccountId,
                        TransactionDate = t.TransactionDate,
                        Description = t.Description,
                        TransactionTypeName = t.TransactionType.Type,
                        CategoryName = t.Category.Category,
                        TransactionKind = "Investment",
                        Quantity = t.Quantity,
                        Price = t.Price,
                        Fees = t.Fees,
                        Commission = t.Commission,
                        OrderType = t.OrderType,
                        SecurityId = t.SecurityId,
                        SecuritySymbol = t.Security.Symbol,
                        SecurityName = t.Security.Name,
                        SecurityIsin = t.Security.ISIN,
                        SecurityTypeName = t.Security.SecurityType,
                        SecurityCurrencySymbol = t.Security.Currency.Symbol
                    })
                    .ToListAsync();

                return Results.Ok(investmentTransactions);
            }

            return Results.NotFound($"Account with ID {id} not found");
        })
        .WithName("GetAccountTransactions")
        .WithDescription("Gets all transactions for the current open account period of a specific account.")
        .Produces<List<TransactionBaseDto>>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status404NotFound);

        // Get recurring transactions (only for cash accounts)
        accountEndpoints.MapGet("/{id}/recurringTransactions", async (FinanceTackerDbContext context, int id) =>
        {
            var accountExists = await context.CashAccounts.AnyAsync(a => a.Id == id);
            if (!accountExists)
                return Results.NotFound($"Cash account with ID {id} not found");

            var recurringTransactions = await context.RecurringTransactions
                .Where(t => t.CashAccountId == id)
                .ToListAsync();
            return Results.Ok(recurringTransactions);
        })
        .WithName("GetAccountRecurringTransactions")
        .WithDescription("Gets all recurring transactions for a specific cash account")
        .Produces<List<RecurringTransaction>>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status404NotFound);

        // Create a new cash account
        accountEndpoints.MapPost("/cash", async (FinanceTackerDbContext context, CreateCashAccountRequestDto request) =>
        {
            // Validate that AccountType and Currency exist
            var accountType = await context.AccountTypes.FindAsync(request.AccountTypeId);
            if (accountType == null)
                return Results.BadRequest($"Account type with ID {request.AccountTypeId} not found.");

            var currency = await context.Currencies.FindAsync(request.CurrencyId);
            if (currency == null)
                return Results.BadRequest($"Currency with ID {request.CurrencyId} not found.");

            var cashAccount = new CashAccount
            {
                Name = request.Name,
                Institution = request.Institution,
                AccountTypeId = request.AccountTypeId,
                CurrencyId = request.CurrencyId,
                IsActive = request.IsActive,
                OverdraftLimit = request.OverdraftLimit
            };

            context.CashAccounts.Add(cashAccount);
            await context.SaveChangesAsync();            // Create initial account period with opening balance
            var accountPeriod = new AccountPeriod
            {
                AccountId = cashAccount.Id,
                Account = cashAccount,
                PeriodStart = DateOnly.FromDateTime(DateTime.UtcNow),
                OpeningBalance = request.InitialBalance,
                PeriodCloseDate = null
            };

            context.AccountPeriods.Add(accountPeriod);
            await context.SaveChangesAsync();

            // Return the created account with calculated balance
            var createdAccount = await context.CashAccounts
                .Include(a => a.AccountType)
                .Include(a => a.Currency)
                .Include(a => a.AccountPeriods)
                .Where(a => a.Id == cashAccount.Id)
                .Select(a => new CashAccountDto
                {
                    Id = a.Id,
                    Name = a.Name,
                    Institution = a.Institution,
                    AccountTypeName = a.AccountType.Type,
                    CurrencySymbol = a.Currency.Symbol,
                    CurrencyDisplaySymbol = a.Currency.DisplaySymbol,
                    IsActive = a.IsActive,
                    AccountKind = "Cash",
                    OverdraftLimit = a.OverdraftLimit,
                    CurrentBalance = a.AccountPeriods
                        .Where(ap => ap.PeriodCloseDate == null)
                        .Select(ap => ap.OpeningBalance + ap.Transactions.OfType<CashTransaction>().Sum(t => t.Amount))
                        .FirstOrDefault()
                })
                .FirstAsync();

            return Results.Created($"/accounts/{cashAccount.Id}", createdAccount);
        })
        .WithName("CreateCashAccount")
        .WithDescription("Creates a new cash account with an initial account period.")
        .Produces<CashAccountDto>(StatusCodes.Status201Created)
        .ProducesProblem(StatusCodes.Status400BadRequest);

        // Create a new investment account
        accountEndpoints.MapPost("/investment", async (FinanceTackerDbContext context, CreateInvestmentAccountRequestDto request) =>
        {
            // Validate that AccountType and Currency exist
            var accountType = await context.AccountTypes.FindAsync(request.AccountTypeId);
            if (accountType == null)
                return Results.BadRequest($"Account type with ID {request.AccountTypeId} not found.");

            var currency = await context.Currencies.FindAsync(request.CurrencyId);
            if (currency == null)
                return Results.BadRequest($"Currency with ID {request.CurrencyId} not found.");

            var investmentAccount = new InvestmentAccount
            {
                Name = request.Name,
                Institution = request.Institution,
                AccountTypeId = request.AccountTypeId,
                CurrencyId = request.CurrencyId,
                IsActive = request.IsActive,
                BrokerAccountNumber = request.BrokerAccountNumber,
                IsTaxAdvantaged = request.IsTaxAdvantaged,
                TaxAdvantageType = request.TaxAdvantageType
            };

            context.InvestmentAccounts.Add(investmentAccount);
            await context.SaveChangesAsync();            // Create initial account period with opening balance
            var accountPeriod = new AccountPeriod
            {
                AccountId = investmentAccount.Id,
                Account = investmentAccount,
                PeriodStart = DateOnly.FromDateTime(DateTime.UtcNow),
                OpeningBalance = request.InitialBalance,
                PeriodCloseDate = null
            };

            context.AccountPeriods.Add(accountPeriod);
            await context.SaveChangesAsync();

            // Return the created account with calculated balance
            var createdAccount = await context.InvestmentAccounts
                .Include(a => a.AccountType)
                .Include(a => a.Currency)
                .Include(a => a.AccountPeriods)
                .Where(a => a.Id == investmentAccount.Id)
                .Select(a => new InvestmentAccountDto
                {
                    Id = a.Id,
                    Name = a.Name,
                    Institution = a.Institution,
                    AccountTypeName = a.AccountType.Type,
                    CurrencySymbol = a.Currency.Symbol,
                    CurrencyDisplaySymbol = a.Currency.DisplaySymbol,
                    IsActive = a.IsActive,
                    AccountKind = "Investment",
                    BrokerAccountNumber = a.BrokerAccountNumber,
                    IsTaxAdvantaged = a.IsTaxAdvantaged,
                    TaxAdvantageType = a.TaxAdvantageType,
                    CurrentBalance = a.AccountPeriods
                        .Where(ap => ap.PeriodCloseDate == null)
                        .Select(ap => ap.OpeningBalance +
                            ap.Transactions.OfType<InvestmentTransaction>().Sum(t => t.Quantity * t.Price))
                        .FirstOrDefault()
                })
                .FirstAsync();

            return Results.Created($"/accounts/{investmentAccount.Id}", createdAccount);
        })
        .WithName("CreateInvestmentAccount")
        .WithDescription("Creates a new investment account with an initial account period.")
        .Produces<InvestmentAccountDto>(StatusCodes.Status201Created)
        .ProducesProblem(StatusCodes.Status400BadRequest);
    }
}
