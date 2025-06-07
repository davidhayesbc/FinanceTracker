using FinanceTracker.Data.Models;
using FinanceTracker.ApiService.Dtos;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace FinanceTracker.ApiService.Endpoints;

public static class CurrencyEndpoints
{
    public static void MapCurrencyEndpoints(this IEndpointRouteBuilder group)
    {
        var currencyEndpoints = group.MapGroup("/currencies")
            .WithTags("Currencies")
            .WithGroupName("Currencies");

        // GET /currencies - Get all currencies
        currencyEndpoints.MapGet("/", async (FinanceTackerDbContext context) =>
        {
            var currencies = await context.Currencies
                .Select(c => new CurrencyDto
                {
                    Id = c.Id,
                    Name = c.Name,
                    Symbol = c.Symbol,
                    DisplaySymbol = c.DisplaySymbol
                })
                .ToListAsync();

            return Results.Ok(currencies);
        })
        .WithName("GetCurrencies")
        .WithSummary("Get all currencies")
        .WithDescription("Retrieves a list of all currencies in the system.")
        .Produces<List<CurrencyDto>>();

        // GET /currencies/{id} - Get currency by ID
        currencyEndpoints.MapGet("/{id:int}", async (int id, FinanceTackerDbContext context) =>
        {
            var currency = await context.Currencies
                .Where(c => c.Id == id)
                .Select(c => new CurrencyDto
                {
                    Id = c.Id,
                    Name = c.Name,
                    Symbol = c.Symbol,
                    DisplaySymbol = c.DisplaySymbol
                })
                .FirstOrDefaultAsync();

            return currency is not null
                ? Results.Ok(currency)
                : Results.NotFound($"Currency with ID {id} not found.");
        })
        .WithName("GetCurrencyById")
        .WithSummary("Get currency by ID")
        .WithDescription("Retrieves a specific currency by its ID.")
        .Produces<CurrencyDto>()
        .Produces(404);

        // POST /currencies - Create new currency
        currencyEndpoints.MapPost("/", async (CreateCurrencyRequestDto request, FinanceTackerDbContext context) =>
        {
            // Check if currency with same symbol already exists
            var existingCurrency = await context.Currencies
                .FirstOrDefaultAsync(c => c.Symbol == request.Symbol);

            if (existingCurrency is not null)
            {
                return Results.Conflict(new
                {
                    error = "A currency with this symbol already exists.",
                    field = "symbol",
                    value = request.Symbol
                });
            }

            var currency = new Currency
            {
                Name = request.Name,
                Symbol = request.Symbol,
                DisplaySymbol = request.DisplaySymbol
            };

            context.Currencies.Add(currency);
            await context.SaveChangesAsync();

            var currencyDto = new CurrencyDto
            {
                Id = currency.Id,
                Name = currency.Name,
                Symbol = currency.Symbol,
                DisplaySymbol = currency.DisplaySymbol
            };

            return Results.Created($"/api/v1/currencies/{currency.Id}", currencyDto);
        })
        .WithName("CreateCurrency")
        .WithSummary("Create a new currency")
        .WithDescription("Creates a new currency in the system.")
        .Produces<CurrencyDto>(201)
        .Produces(400);

        // PUT /currencies/{id} - Update currency
        currencyEndpoints.MapPut("/{id:int}", async (int id, UpdateCurrencyRequestDto request, FinanceTackerDbContext context) =>
        {
            var currency = await context.Currencies.FindAsync(id);
            if (currency is null)
            {
                return Results.NotFound($"Currency with ID {id} not found.");
            }

            // Check if another currency with same symbol exists
            var existingCurrency = await context.Currencies
                .FirstOrDefaultAsync(c => c.Symbol == request.Symbol && c.Id != id);

            if (existingCurrency is not null)
            {
                return Results.BadRequest($"Another currency with symbol '{request.Symbol}' already exists.");
            }

            currency.Name = request.Name;
            currency.Symbol = request.Symbol;
            currency.DisplaySymbol = request.DisplaySymbol;

            await context.SaveChangesAsync();

            var currencyDto = new CurrencyDto
            {
                Id = currency.Id,
                Name = currency.Name,
                Symbol = currency.Symbol,
                DisplaySymbol = currency.DisplaySymbol
            };

            return Results.Ok(currencyDto);
        })
        .WithName("UpdateCurrency")
        .WithSummary("Update a currency")
        .WithDescription("Updates an existing currency.")
        .Produces<CurrencyDto>()
        .Produces(404)
        .Produces(400);

        // DELETE /currencies/{id} - Delete currency
        currencyEndpoints.MapDelete("/{id:int}", async (int id, FinanceTackerDbContext context) =>
        {
            var currency = await context.Currencies
                .Include(c => c.Accounts)
                .Include(c => c.Securities)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (currency is null)
            {
                return Results.NotFound($"Currency with ID {id} not found.");
            }

            // Check if currency is being used by accounts or securities
            if (currency.Accounts.Any() || currency.Securities.Any())
            {
                return Results.BadRequest("Cannot delete currency that is being used by accounts or securities.");
            }

            context.Currencies.Remove(currency);
            await context.SaveChangesAsync();

            return Results.NoContent();
        })
        .WithName("DeleteCurrency")
        .WithSummary("Delete a currency")
        .WithDescription("Deletes a currency if it's not being used by any accounts or securities.")
        .Produces(204)
        .Produces(404)
        .Produces(400);
    }
}
