using FinanceTracker.Data.Models;
using FinanceTracker.ApiService.Dtos;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using System.Linq;

namespace FinanceTracker.ApiService.Endpoints;

public static class AccountTypeEndpoints
{
    public static void MapAccountTypeEndpoints(this IEndpointRouteBuilder group)
    {
        var accountTypeEndpoints = group.MapGroup("/accountTypes")
            .WithTags("Account Types")
            .WithGroupName("Account Types");

        accountTypeEndpoints.MapGet("/", async (FinanceTackerDbContext context) =>
        {
            var types = await context.AccountTypes.ToListAsync();
            return Results.Ok(types);
        })
        .WithName("GetAllAccountTypes")
        .WithDescription("Gets all account types")
        .Produces<List<AccountType>>(StatusCodes.Status200OK);

        accountTypeEndpoints.MapPost("/", async (FinanceTackerDbContext context, CreateAccountTypeRequestDto request) =>
        {
            if (request == null) return Results.BadRequest("Account type data is required");

            var validationResults = new List<ValidationResult>();
            if (!Validator.TryValidateObject(request, new ValidationContext(request), validationResults, true))
            {
                return Results.BadRequest(validationResults);
            }

            // Check if account type already exists
            var existingAccountType = await context.AccountTypes
                .FirstOrDefaultAsync(at => at.Type == request.Type);

            if (existingAccountType != null)
            {
                return Results.Conflict($"Account type '{request.Type}' already exists.");
            }

            var accountType = new AccountType
            {
                Type = request.Type
            };

            context.AccountTypes.Add(accountType);
            await context.SaveChangesAsync();
            return Results.Created($"/api/v1/accountTypes/{accountType.Id}", accountType);
        })
        .WithName("CreateAccountType")
        .WithDescription("Creates a new account type")
        .Produces<AccountType>(StatusCodes.Status201Created)
        .ProducesProblem(StatusCodes.Status400BadRequest);

        // GET /accountTypes/{id} - Get account type by ID
        accountTypeEndpoints.MapGet("/{id:int}", async (int id, FinanceTackerDbContext context) =>
        {
            var accountType = await context.AccountTypes.FindAsync(id);
            if (accountType is null)
            {
                return Results.NotFound($"Account type with ID {id} not found.");
            }

            return Results.Ok(accountType);
        })
        .WithName("GetAccountTypeById")
        .WithDescription("Gets a specific account type by ID")
        .Produces<AccountType>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status404NotFound);

        // PUT /accountTypes/{id} - Update account type
        accountTypeEndpoints.MapPut("/{id:int}", async (int id, UpdateAccountTypeRequestDto request, FinanceTackerDbContext context) =>
        {
            if (request == null) return Results.BadRequest("Account type data is required");

            var validationResults = new List<ValidationResult>();
            if (!Validator.TryValidateObject(request, new ValidationContext(request), validationResults, true))
            {
                return Results.BadRequest(validationResults);
            }

            var accountType = await context.AccountTypes.FindAsync(id);
            if (accountType is null)
            {
                return Results.NotFound($"Account type with ID {id} not found.");
            }

            // Check if another account type with same type exists
            var existingAccountType = await context.AccountTypes
                .FirstOrDefaultAsync(at => at.Type == request.Type && at.Id != id);

            if (existingAccountType is not null)
            {
                return Results.BadRequest($"Another account type with type '{request.Type}' already exists.");
            }

            accountType.Type = request.Type;
            await context.SaveChangesAsync();

            return Results.Ok(accountType);
        })
        .WithName("UpdateAccountType")
        .WithDescription("Updates an existing account type")
        .Produces<AccountType>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status404NotFound)
        .Produces(StatusCodes.Status400BadRequest);

        // DELETE /accountTypes/{id} - Delete account type
        accountTypeEndpoints.MapDelete("/{id:int}", async (int id, FinanceTackerDbContext context) =>
        {
            var accountType = await context.AccountTypes
                .Include(at => at.Accounts)
                .FirstOrDefaultAsync(at => at.Id == id);

            if (accountType is null)
            {
                return Results.NotFound($"Account type with ID {id} not found.");
            }

            // Check if account type is being used by accounts
            if (accountType.Accounts.Any())
            {
                return Results.BadRequest("Cannot delete account type that is being used by accounts.");
            }

            context.AccountTypes.Remove(accountType);
            await context.SaveChangesAsync();

            return Results.NoContent();
        })
        .WithName("DeleteAccountType")
        .WithDescription("Deletes an account type if it's not being used by any accounts")
        .Produces(StatusCodes.Status204NoContent)
        .Produces(StatusCodes.Status404NotFound)
        .Produces(StatusCodes.Status400BadRequest);
    }
}
