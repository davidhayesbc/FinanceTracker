using FinanceTracker.Data.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

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

        accountTypeEndpoints.MapPost("/", async (FinanceTackerDbContext context, AccountType accountType) =>
        {
            if (accountType == null) return Results.BadRequest("Account type data is required");

            var validationResults = new List<ValidationResult>();
            if (!Validator.TryValidateObject(accountType, new ValidationContext(accountType), validationResults, true))
            {
                return Results.BadRequest(validationResults);
            }

            context.AccountTypes.Add(accountType);
            await context.SaveChangesAsync();
            return Results.Created($"/api/v1/accountTypes/{accountType.Id}", accountType);
        })
        .WithName("CreateAccountType")
        .WithDescription("Creates a new account type")
        .Produces<AccountType>(StatusCodes.Status201Created)
        .ProducesProblem(StatusCodes.Status400BadRequest);
    }
}
