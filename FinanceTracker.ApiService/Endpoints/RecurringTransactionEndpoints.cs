using FinanceTracker.Data.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace FinanceTracker.ApiService.Endpoints;

public static class RecurringTransactionEndpoints
{
    public static void MapRecurringTransactionEndpoints(this IEndpointRouteBuilder group)
    {
        var recurringTransactionsEndpoints = group.MapGroup("/recurringTransactions")
            .WithTags("Recurring Transactions")
            .WithGroupName("Recurring Transactions");

        recurringTransactionsEndpoints.MapGet("/", async (FinanceTackerDbContext context) =>
        {
            var recurringTransactions = await context.RecurringTransactions.ToListAsync();
            return Results.Ok(recurringTransactions);
        })
        .WithName("GetAllRecurringTransactions")
        .WithDescription("Gets all recurring transactions")
        .Produces<List<RecurringTransaction>>(StatusCodes.Status200OK);

        recurringTransactionsEndpoints.MapPost("/", async (FinanceTackerDbContext context, RecurringTransaction recurringTransaction) =>
        {
            if (recurringTransaction == null) return Results.BadRequest("Recurring transaction data is required");

            var validationResults = new List<ValidationResult>();
            if (!Validator.TryValidateObject(recurringTransaction, new ValidationContext(recurringTransaction), validationResults, true))
            {
                return Results.BadRequest(validationResults);
            }

            // Validate that the account exists
            if (!await context.Accounts.AnyAsync(a => a.Id == recurringTransaction.AccountId))
            {
                return Results.BadRequest($"Account with ID {recurringTransaction.AccountId} does not exist");
            }

            context.RecurringTransactions.Add(recurringTransaction);
            await context.SaveChangesAsync();
            return Results.Created($"/api/v1/recurringTransactions/{recurringTransaction.Id}", recurringTransaction);
        })
        .WithName("CreateRecurringTransaction")
        .WithDescription("Creates a new recurring transaction")
        .Produces<RecurringTransaction>(StatusCodes.Status201Created)
        .ProducesProblem(StatusCodes.Status400BadRequest);
    }
}
