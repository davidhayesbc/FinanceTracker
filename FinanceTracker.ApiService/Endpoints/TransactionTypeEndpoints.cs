using FinanceTracker.Data.Models;
using FinanceTracker.ApiService.Dtos;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace FinanceTracker.ApiService.Endpoints;

public static class TransactionTypeEndpoints
{
    public static void MapTransactionTypeEndpoints(this IEndpointRouteBuilder group)
    {
        var transactionTypeEndpoints = group.MapGroup("/transactionTypes")
            .WithTags("Transaction Types")
            .WithGroupName("Transaction Types");

        transactionTypeEndpoints.MapGet("/", async (FinanceTackerDbContext context) =>
        {
            var types = await context.TransactionTypes.ToListAsync();
            return Results.Ok(types);
        })
        .WithName("GetAllTransactionTypes")
        .WithDescription("Gets all transaction types")
        .Produces<List<TransactionType>>(StatusCodes.Status200OK);

        transactionTypeEndpoints.MapPost("/", async (FinanceTackerDbContext context, CreateTransactionTypeRequestDto request) =>
        {
            if (request == null) return Results.BadRequest("Transaction type data is required");

            var validationResults = new List<ValidationResult>();
            if (!Validator.TryValidateObject(request, new ValidationContext(request), validationResults, true))
            {
                return Results.BadRequest(validationResults);
            }

            // Check if transaction type already exists
            var existingTransactionType = await context.TransactionTypes
                .FirstOrDefaultAsync(tt => tt.Type == request.Type);

            if (existingTransactionType != null)
            {
                return Results.Conflict($"Transaction type '{request.Type}' already exists.");
            }

            var transactionType = new TransactionType
            {
                Type = request.Type
            };

            context.TransactionTypes.Add(transactionType);
            await context.SaveChangesAsync();
            return Results.Created($"/api/v1/transactionTypes/{transactionType.Id}", transactionType);
        })
        .WithName("CreateTransactionType")
        .WithDescription("Creates a new transaction type")
        .Produces<TransactionType>(StatusCodes.Status201Created)
        .ProducesProblem(StatusCodes.Status400BadRequest);
    }
}
