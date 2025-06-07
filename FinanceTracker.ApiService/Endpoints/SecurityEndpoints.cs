using FinanceTracker.Data.Models;
using FinanceTracker.ApiService.Dtos;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace FinanceTracker.ApiService.Endpoints;

public static class SecurityEndpoints
{
    public static void MapSecurityEndpoints(this IEndpointRouteBuilder group)
    {
        var securityEndpoints = group.MapGroup("/securities")
            .WithTags("Securities")
            .WithGroupName("Securities");

        // GET /securities - Get all securities
        securityEndpoints.MapGet("/", async (FinanceTackerDbContext context) =>
        {
            var securities = await context.Securities
                .Include(s => s.Currency)
                .Select(s => new SecurityDto
                {
                    Id = s.Id,
                    Symbol = s.Symbol,
                    Name = s.Name,
                    ISIN = s.ISIN,
                    CurrencyId = s.CurrencyId,
                    CurrencySymbol = s.Currency.Symbol,
                    CurrencyDisplaySymbol = s.Currency.DisplaySymbol,
                    SecurityType = s.SecurityType
                })
                .ToListAsync();

            return Results.Ok(securities);
        })
        .WithName("GetSecurities")
        .WithSummary("Get all securities")
        .WithDescription("Retrieves a list of all securities in the system.")
        .Produces<List<SecurityDto>>();

        // GET /securities/{id} - Get security by ID
        securityEndpoints.MapGet("/{id:int}", async (int id, FinanceTackerDbContext context) =>
        {
            var security = await context.Securities
                .Include(s => s.Currency)
                .Where(s => s.Id == id)
                .Select(s => new SecurityDto
                {
                    Id = s.Id,
                    Symbol = s.Symbol,
                    Name = s.Name,
                    ISIN = s.ISIN,
                    CurrencyId = s.CurrencyId,
                    CurrencySymbol = s.Currency.Symbol,
                    CurrencyDisplaySymbol = s.Currency.DisplaySymbol,
                    SecurityType = s.SecurityType
                })
                .FirstOrDefaultAsync();

            return security is not null
                ? Results.Ok(security)
                : Results.NotFound($"Security with ID {id} not found.");
        })
        .WithName("GetSecurityById")
        .WithSummary("Get security by ID")
        .WithDescription("Retrieves a specific security by its ID.")
        .Produces<SecurityDto>()
        .Produces(404);

        // GET /securities/by-symbol/{symbol} - Get security by symbol
        securityEndpoints.MapGet("/by-symbol/{symbol}", async (string symbol, FinanceTackerDbContext context) =>
        {
            var security = await context.Securities
                .Include(s => s.Currency)
                .Where(s => s.Symbol == symbol)
                .Select(s => new SecurityDto
                {
                    Id = s.Id,
                    Symbol = s.Symbol,
                    Name = s.Name,
                    ISIN = s.ISIN,
                    CurrencyId = s.CurrencyId,
                    CurrencySymbol = s.Currency.Symbol,
                    CurrencyDisplaySymbol = s.Currency.DisplaySymbol,
                    SecurityType = s.SecurityType
                })
                .FirstOrDefaultAsync();

            return security is not null
                ? Results.Ok(security)
                : Results.NotFound($"Security with symbol '{symbol}' not found.");
        })
        .WithName("GetSecurityBySymbol")
        .WithSummary("Get security by symbol")
        .WithDescription("Retrieves a specific security by its symbol.")
        .Produces<SecurityDto>()
        .Produces(404);

        // POST /securities - Create new security
        securityEndpoints.MapPost("/", async (CreateSecurityRequestDto request, FinanceTackerDbContext context) =>
        {
            // Check if security with same symbol already exists
            var existingSecurity = await context.Securities
                .FirstOrDefaultAsync(s => s.Symbol == request.Symbol);

            if (existingSecurity is not null)
            {
                return Results.Conflict($"Security with symbol '{request.Symbol}' already exists.");
            }

            // Verify currency exists
            var currencyExists = await context.Currencies
                .AnyAsync(c => c.Id == request.CurrencyId);

            if (!currencyExists)
            {
                return Results.BadRequest($"Currency with ID {request.CurrencyId} does not exist.");
            }

            var security = new Security
            {
                Symbol = request.Symbol,
                Name = request.Name,
                ISIN = request.ISIN,
                CurrencyId = request.CurrencyId,
                SecurityType = request.SecurityType
            };

            context.Securities.Add(security);
            await context.SaveChangesAsync();

            // Fetch the created security with currency details
            var createdSecurity = await context.Securities
                .Include(s => s.Currency)
                .Where(s => s.Id == security.Id)
                .Select(s => new SecurityDto
                {
                    Id = s.Id,
                    Symbol = s.Symbol,
                    Name = s.Name,
                    ISIN = s.ISIN,
                    CurrencyId = s.CurrencyId,
                    CurrencySymbol = s.Currency.Symbol,
                    CurrencyDisplaySymbol = s.Currency.DisplaySymbol,
                    SecurityType = s.SecurityType
                })
                .FirstAsync();

            return Results.Created($"/api/v1/securities/{security.Id}", createdSecurity);
        })
        .WithName("CreateSecurity")
        .WithSummary("Create a new security")
        .WithDescription("Creates a new security in the system.")
        .Produces<SecurityDto>(201)
        .Produces(400)
        .Produces(409);

        // PUT /securities/{id} - Update security
        securityEndpoints.MapPut("/{id:int}", async (int id, UpdateSecurityRequestDto request, FinanceTackerDbContext context) =>
        {
            var security = await context.Securities.FindAsync(id);
            if (security is null)
            {
                return Results.NotFound($"Security with ID {id} not found.");
            }

            // Check if another security with same symbol exists
            var existingSecurity = await context.Securities
                .FirstOrDefaultAsync(s => s.Symbol == request.Symbol && s.Id != id);

            if (existingSecurity is not null)
            {
                return Results.BadRequest($"Another security with symbol '{request.Symbol}' already exists.");
            }

            // Verify currency exists
            var currencyExists = await context.Currencies
                .AnyAsync(c => c.Id == request.CurrencyId);

            if (!currencyExists)
            {
                return Results.BadRequest($"Currency with ID {request.CurrencyId} does not exist.");
            }

            security.Symbol = request.Symbol;
            security.Name = request.Name;
            security.ISIN = request.ISIN;
            security.CurrencyId = request.CurrencyId;
            security.SecurityType = request.SecurityType;

            await context.SaveChangesAsync();

            // Fetch the updated security with currency details
            var updatedSecurity = await context.Securities
                .Include(s => s.Currency)
                .Where(s => s.Id == security.Id)
                .Select(s => new SecurityDto
                {
                    Id = s.Id,
                    Symbol = s.Symbol,
                    Name = s.Name,
                    ISIN = s.ISIN,
                    CurrencyId = s.CurrencyId,
                    CurrencySymbol = s.Currency.Symbol,
                    CurrencyDisplaySymbol = s.Currency.DisplaySymbol,
                    SecurityType = s.SecurityType
                })
                .FirstAsync();

            return Results.Ok(updatedSecurity);
        })
        .WithName("UpdateSecurity")
        .WithSummary("Update a security")
        .WithDescription("Updates an existing security.")
        .Produces<SecurityDto>()
        .Produces(404)
        .Produces(400);

        // DELETE /securities/{id} - Delete security
        securityEndpoints.MapDelete("/{id:int}", async (int id, FinanceTackerDbContext context) =>
        {
            var security = await context.Securities
                .Include(s => s.InvestmentTransactions)
                .Include(s => s.Prices)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (security is null)
            {
                return Results.NotFound($"Security with ID {id} not found.");
            }

            // Check if security is being used by investment transactions or has price data
            if (security.InvestmentTransactions.Any() || security.Prices.Any())
            {
                return Results.BadRequest("Cannot delete security that has associated transactions or price data.");
            }

            context.Securities.Remove(security);
            await context.SaveChangesAsync();

            return Results.NoContent();
        })
        .WithName("DeleteSecurity")
        .WithSummary("Delete a security")
        .WithDescription("Deletes a security if it has no associated transactions or price data.")
        .Produces(204)
        .Produces(404)
        .Produces(400);

        // GET /securities/types - Get distinct security types
        securityEndpoints.MapGet("/types", async (FinanceTackerDbContext context) =>
        {
            var securityTypes = await context.Securities
                .Select(s => s.SecurityType)
                .Distinct()
                .OrderBy(st => st)
                .ToListAsync();

            return Results.Ok(securityTypes);
        })
        .WithName("GetSecurityTypes")
        .WithSummary("Get security types")
        .WithDescription("Retrieves a list of all distinct security types in the system.")
        .Produces<List<string>>();
    }
}
