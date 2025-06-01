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

public static class TransactionCategoryEndpoints
{
    public static void MapTransactionCategoryEndpoints(this IEndpointRouteBuilder group)
    {
        var transactionCategoryEndpoints = group.MapGroup("/transactionCategories")
            .WithTags("Transaction Categories")
            .WithGroupName("Transaction Categories");

        transactionCategoryEndpoints.MapGet("/", async (FinanceTackerDbContext context) =>
        {
            var categories = await context.TransactionCategories.ToListAsync();
            return Results.Ok(categories);
        })
        .WithName("GetAllTransactionCategories")
        .WithDescription("Gets all transaction categories")
        .Produces<List<TransactionCategory>>(StatusCodes.Status200OK);

        transactionCategoryEndpoints.MapPost("/", async (FinanceTackerDbContext context, CreateTransactionCategoryRequestDto request) =>
        {
            if (request == null) return Results.BadRequest("Transaction category data is required");

            var validationResults = new List<ValidationResult>();
            if (!Validator.TryValidateObject(request, new ValidationContext(request), validationResults, true))
            {
                return Results.BadRequest(validationResults);
            }

            // Check if transaction category already exists
            var existingCategory = await context.TransactionCategories
                .FirstOrDefaultAsync(tc => tc.Category == request.Category);

            if (existingCategory != null)
            {
                return Results.Conflict($"Transaction category '{request.Category}' already exists.");
            }

            var transactionCategory = new TransactionCategory
            {
                Category = request.Category,
                Description = request.Description
            };

            context.TransactionCategories.Add(transactionCategory);
            await context.SaveChangesAsync();
            return Results.Created($"/api/v1/transactionCategories/{transactionCategory.Id}", transactionCategory);
        })
        .WithName("CreateTransactionCategory")
        .WithDescription("Creates a new transaction category")
        .Produces<TransactionCategory>(StatusCodes.Status201Created)
        .ProducesProblem(StatusCodes.Status400BadRequest);
    }
}
