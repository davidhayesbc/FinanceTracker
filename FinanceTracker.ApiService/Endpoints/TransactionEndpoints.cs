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

namespace FinanceTracker.ApiService.Endpoints;

public static class TransactionEndpoints
{
    public static void MapTransactionEndpoints(this IEndpointRouteBuilder group)
    {
        var transactionEndpoints = group.MapGroup("/transactions")
            .WithTags("Transactions")
            .WithGroupName("Transactions");

        transactionEndpoints.MapGet("/", async (FinanceTackerDbContext context) =>
        {
            var transactions = await context.Transactions
                .Include(t => t.Security)
                    .ThenInclude(s => s!.Currency)
                .Include(t => t.TransactionType)
                .Include(t => t.AccountPeriod)
                .Select(t => new TransactionDto
                {
                    Id = t.Id,
                    AccountId = t.AccountPeriod.AccountId,
                    TransactionDate = t.TransactionDate,
                    Description = t.Description,
                    Quantity = t.Quantity,
                    OriginalCost = t.OriginalCost,
                    SecurityId = t.SecurityId,
                    SecuritySymbol = t.Security != null ? t.Security.Symbol : null,
                    SecurityName = t.Security != null ? t.Security.Name : null,
                    SecurityIsin = t.Security != null ? t.Security.ISIN : null,
                    SecurityTypeName = t.Security != null ? t.Security.SecurityType : null,
                    SecurityCurrencySymbol = t.Security != null && t.Security.Currency != null ? t.Security.Currency.Symbol : null,
                    TransactionTypeName = t.TransactionType.Type,
                })
                .ToListAsync();
            return Results.Ok(transactions);
        })
        .WithName("GetAllTransactions")
        .WithDescription("Gets all transactions with flattened security and type information.")
        .Produces<List<TransactionDto>>(StatusCodes.Status200OK);

        transactionEndpoints.MapGet("/{id}", async (FinanceTackerDbContext context, int id) =>
        {
            var transaction = await context.Transactions
                .Where(t => t.Id == id)
                .Include(t => t.Security)
                    .ThenInclude(s => s!.Currency)
                .Include(t => t.TransactionType)
                .Include(t => t.AccountPeriod)
                .Select(t => new TransactionDto
                {
                    Id = t.Id,
                    AccountId = t.AccountPeriod.AccountId,
                    TransactionDate = t.TransactionDate,
                    Description = t.Description,
                    Quantity = t.Quantity,
                    OriginalCost = t.OriginalCost,
                    SecurityId = t.SecurityId,
                    SecuritySymbol = t.Security != null ? t.Security.Symbol : null,
                    SecurityName = t.Security != null ? t.Security.Name : null,
                    SecurityIsin = t.Security != null ? t.Security.ISIN : null,
                    SecurityTypeName = t.Security != null ? t.Security.SecurityType : null,
                    SecurityCurrencySymbol = t.Security != null && t.Security.Currency != null ? t.Security.Currency.Symbol : null,
                    TransactionTypeName = t.TransactionType.Type,
                })
                .FirstOrDefaultAsync();

            if (transaction == null)
            {
                return Results.NotFound($"Transaction with ID {id} not found.");
            }
            return Results.Ok(transaction);
        })
        .WithName("GetTransactionById")
        .WithDescription("Gets a transaction by its ID with flattened security and type information.")
        .Produces<TransactionDto>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status404NotFound);

        transactionEndpoints.MapGet("/{id}/transactionSplits", async (FinanceTackerDbContext context, int id) =>
        {
            var transactionExists = await context.Transactions.AnyAsync(t => t.Id == id);
            if (!transactionExists)
                return Results.NotFound($"Transaction with ID {id} not found");

            var transactionSplits = await context.TransactionSplits.Where(t => t.TransactionId == id).ToListAsync();
            return Results.Ok(transactionSplits);
        })
        .WithName("GetTransactionSplits")
        .WithDescription("Gets all splits for a specific transaction")
        .Produces<List<TransactionSplit>>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status404NotFound);

        transactionEndpoints.MapPost("/", async (FinanceTackerDbContext context, CreateTransactionRequestDto transactionDto) =>
        {
            if (transactionDto == null) return Results.BadRequest("Transaction data is required");

            var validationResults = new List<ValidationResult>();
            if (!Validator.TryValidateObject(transactionDto, new ValidationContext(transactionDto), validationResults, true))
            {
                return Results.BadRequest(validationResults);
            }

            // Validate that the account exists by checking AccountPeriod's AccountId
            var accountPeriod = await context.AccountPeriods.FindAsync(transactionDto.AccountPeriodId);
            if (accountPeriod == null || !await context.Accounts.AnyAsync(a => a.Id == accountPeriod.AccountId))
            {
                return Results.BadRequest($"Account with ID {accountPeriod?.AccountId} associated with AccountPeriodId {transactionDto.AccountPeriodId} does not exist or AccountPeriodId is invalid.");
            }

            // Validate SecurityId
            if (!await context.Securities.AnyAsync(s => s.Id == transactionDto.SecurityId))
            {
                return Results.BadRequest($"Security with ID {transactionDto.SecurityId} does not exist.");
            }

            // Validate TransactionTypeId
            if (!await context.TransactionTypes.AnyAsync(tt => tt.Id == transactionDto.TransactionTypeId))
            {
                return Results.BadRequest($"TransactionType with ID {transactionDto.TransactionTypeId} does not exist.");
            }

            // Validate CategoryId
            if (!await context.TransactionCategories.AnyAsync(tc => tc.Id == transactionDto.CategoryId))
            {
                return Results.BadRequest($"TransactionCategory with ID {transactionDto.CategoryId} does not exist.");
            }

            var transaction = new Transaction
            {
                TransactionDate = transactionDto.TransactionDate,
                Quantity = transactionDto.Quantity,
                OriginalCost = transactionDto.OriginalCost,
                Description = transactionDto.Description,
                TransactionTypeId = transactionDto.TransactionTypeId,
                CategoryId = transactionDto.CategoryId,
                AccountPeriodId = transactionDto.AccountPeriodId,
                SecurityId = transactionDto.SecurityId
            };

            context.Transactions.Add(transaction);
            await context.SaveChangesAsync();

            // Map to TransactionDto for the response
            var createdTransactionDto = await context.Transactions
                .Where(t => t.Id == transaction.Id)
                .Include(t => t.Security)
                    .ThenInclude(s => s!.Currency)
                .Include(t => t.TransactionType)
                .Include(t => t.AccountPeriod)
                .Select(t => new TransactionDto
                {
                    Id = t.Id,
                    AccountId = t.AccountPeriod.AccountId,
                    TransactionDate = t.TransactionDate,
                    Description = t.Description,
                    Quantity = t.Quantity,
                    OriginalCost = t.OriginalCost,
                    SecurityId = t.SecurityId,
                    SecuritySymbol = t.Security != null ? t.Security.Symbol : null,
                    SecurityName = t.Security != null ? t.Security.Name : null,
                    SecurityIsin = t.Security != null ? t.Security.ISIN : null,
                    SecurityTypeName = t.Security != null ? t.Security.SecurityType : null,
                    SecurityCurrencySymbol = t.Security != null && t.Security.Currency != null ? t.Security.Currency.Symbol : null,
                    TransactionTypeName = t.TransactionType.Type,
                })
                .FirstOrDefaultAsync();

            return Results.Created($"/api/v1/transactions/{transaction.Id}", createdTransactionDto);
        })
        .WithName("CreateTransaction")
        .WithDescription("Creates a new transaction")
        .Produces<TransactionDto>(StatusCodes.Status201Created)
        .ProducesProblem(StatusCodes.Status400BadRequest);
    }
}
