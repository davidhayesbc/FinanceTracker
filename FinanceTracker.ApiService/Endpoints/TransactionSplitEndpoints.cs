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

public static class TransactionSplitEndpoints
{
    public static void MapTransactionSplitEndpoints(this IEndpointRouteBuilder group)
    {
        var transactionSplitEndpoints = group.MapGroup("/transactionSplits")
            .WithTags("Transaction Splits")
            .WithGroupName("Transaction Splits");

        transactionSplitEndpoints.MapGet("/", async (FinanceTackerDbContext context) =>
        {
            var splits = await context.TransactionSplits
                .Include(ts => ts.Category)
                .Select(ts => new TransactionSplitDto
                {
                    Id = ts.Id,
                    CashTransactionId = ts.CashTransactionId,
                    CategoryId = ts.CategoryId,
                    CategoryName = ts.Category.Category, // Changed from ts.Category.Name
                    Amount = ts.Amount
                })
                .ToListAsync();
            return Results.Ok(splits);
        })
        .WithName("GetAllTransactionSplits")
        .WithDescription("Gets all transaction splits with category name.")
        .Produces<List<TransactionSplitDto>>(StatusCodes.Status200OK);

        transactionSplitEndpoints.MapPost("/", async (FinanceTackerDbContext context, CreateTransactionSplitRequestDto transactionSplitDto) =>
        {
            if (transactionSplitDto == null) return Results.BadRequest("Transaction split data is required.");

            var validationResults = new List<ValidationResult>();
            if (!Validator.TryValidateObject(transactionSplitDto, new ValidationContext(transactionSplitDto), validationResults, true))
            {
                return Results.BadRequest(validationResults);
            }

            // Validate that the cash transaction exists
            if (!await context.CashTransactions.AnyAsync(t => t.Id == transactionSplitDto.CashTransactionId))
            {
                return Results.BadRequest($"Cash transaction with ID {transactionSplitDto.CashTransactionId} does not exist.");
            }

            // Validate that the category exists
            if (!await context.TransactionCategories.AnyAsync(tc => tc.Id == transactionSplitDto.CategoryId))
            {
                return Results.BadRequest($"Category with ID {transactionSplitDto.CategoryId} does not exist.");
            }

            var transactionSplit = new TransactionSplit
            {
                CashTransactionId = transactionSplitDto.CashTransactionId,
                CategoryId = transactionSplitDto.CategoryId,
                Amount = transactionSplitDto.Amount
            };

            context.TransactionSplits.Add(transactionSplit);
            await context.SaveChangesAsync();

            // Map to TransactionSplitDto for the response
            var createdSplitDto = new TransactionSplitDto
            {
                Id = transactionSplit.Id,
                CashTransactionId = transactionSplit.CashTransactionId,
                CategoryId = transactionSplit.CategoryId,
                CategoryName = (await context.TransactionCategories.FindAsync(transactionSplit.CategoryId))?.Category ?? string.Empty, // Changed from .Name
                Amount = transactionSplit.Amount
            };

            return Results.Created($"/api/v1/transactionSplits/{transactionSplit.Id}", createdSplitDto);
        })
        .WithName("CreateTransactionSplit")
        .WithDescription("Creates a new transaction split")
        .Produces<TransactionSplitDto>(StatusCodes.Status201Created)
        .ProducesProblem(StatusCodes.Status400BadRequest);
    }
}
