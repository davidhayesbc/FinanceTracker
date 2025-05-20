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

        accountEndpoints.MapGet("/", async (FinanceTackerDbContext context) =>
        {
            var accountDtos = await context.Accounts
                .Include(a => a.AccountType)
                .Include(a => a.Currency)
                .Include(a => a.AccountPeriods)
                    .ThenInclude(ap => ap.Transactions)
                        .ThenInclude(t => t.Security)
                            .ThenInclude(s => s.Currency)
                .Select(a => new AccountDto
                {
                    Id = a.Id,
                    Name = a.Name,
                    Institution = a.Institution,
                    AccountTypeName = a.AccountType != null ? a.AccountType.Type : string.Empty,
                    CurrencySymbol = a.Currency != null ? a.Currency.Symbol : string.Empty,
                    CurrencyDisplaySymbol = a.Currency != null ? a.Currency.DisplaySymbol : string.Empty,
                    CurrentBalance = a.AccountPeriods
                        .Where(ap => ap.PeriodCloseDate == null)
                        .Select(ap => ap.OpeningBalance +
                            ap.Transactions.Sum(t =>
                                t.Quantity *
                                (context.Prices
                                    .Where(price => price.SecurityId == t.SecurityId && DateOnly.FromDateTime(price.Date) <= t.TransactionDate)
                                    .OrderByDescending(price => price.Date)
                                    .Select(price => price.ClosePrice)
                                    .FirstOrDefault()) *
                                (t.Security.CurrencyId == a.CurrencyId ? 1.0m :
                                    (context.FxRates
                                        .Where(fx => fx.FromCurrencyId == t.Security.CurrencyId && fx.ToCurrencyId == a.CurrencyId && fx.Date <= t.TransactionDate)
                                        .OrderByDescending(fx => fx.Date)
                                        .Select(fx => (decimal?)fx.Rate)
                                        .FirstOrDefault() ?? 1.0m))
                            )
                        ).FirstOrDefault()
                })
                .ToListAsync();

            return Results.Ok(accountDtos);
        })
        .WithName("GetAllAccounts")
        .WithDescription("Gets all accounts with flattened related data, including calculated current balance from the open account period.")
        .Produces<List<AccountDto>>(StatusCodes.Status200OK);

        accountEndpoints.MapGet("/{id}", async (FinanceTackerDbContext context, int id) =>
        {
            var accountDto = await context.Accounts
                .Where(a => a.Id == id)
                .Include(a => a.AccountType)
                .Include(a => a.Currency)
                .Include(a => a.AccountPeriods)
                    .ThenInclude(ap => ap.Transactions)
                        .ThenInclude(t => t.Security)
                            .ThenInclude(s => s.Currency)
                .Select(a => new AccountDto
                {
                    Id = a.Id,
                    Name = a.Name,
                    Institution = a.Institution,
                    AccountTypeName = a.AccountType != null ? a.AccountType.Type : string.Empty,
                    CurrencySymbol = a.Currency != null ? a.Currency.Symbol : string.Empty,
                    CurrencyDisplaySymbol = a.Currency != null ? a.Currency.DisplaySymbol : string.Empty,
                    CurrentBalance = a.AccountPeriods
                        .Where(ap => ap.PeriodCloseDate == null)
                        .Select(ap => ap.OpeningBalance +
                            ap.Transactions.Sum(t =>
                                t.Quantity *
                                (context.Prices
                                    .Where(price => price.SecurityId == t.SecurityId && DateOnly.FromDateTime(price.Date) <= t.TransactionDate)
                                    .OrderByDescending(price => price.Date)
                                    .Select(price => price.ClosePrice)
                                    .FirstOrDefault()) *
                                (t.Security.CurrencyId == a.CurrencyId ? 1.0m :
                                    (context.FxRates
                                        .Where(fx => fx.FromCurrencyId == t.Security.CurrencyId && fx.ToCurrencyId == a.CurrencyId && fx.Date <= t.TransactionDate)
                                        .OrderByDescending(fx => fx.Date)
                                        .Select(fx => (decimal?)fx.Rate)
                                        .FirstOrDefault() ?? 1.0m))
                            )
                        ).FirstOrDefault()
                })
                .FirstOrDefaultAsync();

            if (accountDto == null)
            {
                return Results.NotFound($"Account with ID {id} not found.");
            }

            return Results.Ok(accountDto);
        })
        .WithName("GetAccountById")
        .WithDescription("Gets an account by its ID, including calculated current balance from the open account period.")
        .Produces<AccountDto>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status404NotFound);

        accountEndpoints.MapGet("/{id}/transactions", async (FinanceTackerDbContext context, int id) =>
        {
            var accountExists = await context.Accounts.AnyAsync(a => a.Id == id);
            if (!accountExists)
            {
                return Results.NotFound($"Account with ID {id} not found");
            }

            var currentPeriodWithTransactions = await context.AccountPeriods
                .Include(ap => ap.Transactions)
                .Where(ap => ap.AccountId == id && ap.PeriodCloseDate == null)
                .FirstOrDefaultAsync();

            if (currentPeriodWithTransactions == null)
            {
                // Account exists, but no current open period found
                return Results.Ok(new List<Transaction>());
            }

            return Results.Ok(currentPeriodWithTransactions.Transactions.ToList());
        })
        .WithName("GetAccountTransactions")
        .WithDescription("Gets all transactions for the current open account period of a specific account.")
        .Produces<List<Transaction>>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status404NotFound);

        accountEndpoints.MapGet("/{id}/recurringTransactions", async (FinanceTackerDbContext context, int id) =>
        {
            var accountExists = await context.Accounts.AnyAsync(a => a.Id == id);
            if (!accountExists)
                return Results.NotFound($"Account with ID {id} not found");

            var recurringTransactions = await context.RecurringTransactions.Where(t => t.AccountId == id).ToListAsync();
            return Results.Ok(recurringTransactions);
        })
        .WithName("GetAccountRecurringTransactions")
        .WithDescription("Gets all recurring transactions for a specific account")
        .Produces<List<RecurringTransaction>>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status404NotFound);

        accountEndpoints.MapPost("/", async (FinanceTackerDbContext context, Account account) =>
        {
            if (account == null) return Results.BadRequest("Account data is required");

            var validationResults = new List<ValidationResult>();
            if (!Validator.TryValidateObject(account, new ValidationContext(account), validationResults, true))
            {
                return Results.BadRequest(validationResults);
            }

            context.Accounts.Add(account);
            await context.SaveChangesAsync();
            return Results.Created($"/api/v1/accounts/{account.Id}", account);
        })
        .WithName("CreateAccount")
        .WithDescription("Creates a new account")
        .Produces<Account>(StatusCodes.Status201Created)
        .ProducesProblem(StatusCodes.Status400BadRequest);
    }
}
