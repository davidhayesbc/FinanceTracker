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

        // Cash Transaction Endpoints
        transactionEndpoints.MapGet("/cash", async (FinanceTackerDbContext context) =>
        {
            var cashTransactions = await context.CashTransactions
                .Include(t => t.TransactionType)
                .Include(t => t.AccountPeriod)
                .Select(t => new CashTransactionDto
                {
                    Id = t.Id,
                    TransactionDate = t.TransactionDate,
                    Description = t.Description,
                    Amount = t.Amount,
                    TransferToCashAccountId = t.TransferToCashAccountId,
                    TransactionTypeId = t.TransactionTypeId,
                    TransactionTypeName = t.TransactionType.Type,
                    AccountPeriodId = t.AccountPeriodId
                })
                .ToListAsync();
            return Results.Ok(cashTransactions);
        })
        .WithName("GetAllCashTransactions")
        .WithDescription("Gets all cash transactions.")
        .Produces<List<CashTransactionDto>>(StatusCodes.Status200OK);

        // Investment Transaction Endpoints
        transactionEndpoints.MapGet("/investment", async (FinanceTackerDbContext context) =>
        {
            var investmentTransactions = await context.InvestmentTransactions
                .Include(t => t.Security)
                    .ThenInclude(s => s.Currency)
                .Include(t => t.TransactionType)
                .Include(t => t.AccountPeriod)
                .Select(t => new InvestmentTransactionDto
                {
                    Id = t.Id,
                    TransactionDate = t.TransactionDate,
                    Description = t.Description,
                    Quantity = t.Quantity,
                    Price = t.Price,
                    Fees = t.Fees,
                    Commission = t.Commission,
                    OrderType = t.OrderType,
                    SecurityId = t.SecurityId,
                    SecuritySymbol = t.Security.Symbol,
                    SecurityName = t.Security.Name,
                    SecurityIsin = t.Security.ISIN,
                    SecurityTypeName = t.Security.SecurityType,
                    SecurityCurrencySymbol = t.Security.Currency.Symbol,
                    TransactionTypeId = t.TransactionTypeId,
                    TransactionTypeName = t.TransactionType.Type,
                    AccountPeriodId = t.AccountPeriodId
                })
                .ToListAsync();
            return Results.Ok(investmentTransactions);
        })
        .WithName("GetAllInvestmentTransactions")
        .WithDescription("Gets all investment transactions.")
        .Produces<List<InvestmentTransactionDto>>(StatusCodes.Status200OK);

        // Create Cash Transaction
        transactionEndpoints.MapPost("/cash", async (FinanceTackerDbContext context, CreateCashTransactionRequestDto cashTransactionDto) =>
        {
            if (cashTransactionDto == null) return Results.BadRequest("Cash transaction data is required");

            var validationResults = new List<ValidationResult>();
            if (!Validator.TryValidateObject(cashTransactionDto, new ValidationContext(cashTransactionDto), validationResults, true))
            {
                return Results.BadRequest(validationResults);
            }

            // Create the cash transaction
            var cashTransaction = new CashTransaction
            {
                TransactionDate = cashTransactionDto.TransactionDate,
                Description = cashTransactionDto.Description,
                Amount = cashTransactionDto.Amount,
                TransferToCashAccountId = cashTransactionDto.TransferToCashAccountId,
                TransactionTypeId = cashTransactionDto.TransactionTypeId,
                AccountPeriodId = cashTransactionDto.AccountPeriodId
            };

            context.CashTransactions.Add(cashTransaction);
            await context.SaveChangesAsync();
            return Results.Created($"/api/v1/transactions/cash/{cashTransaction.Id}", cashTransaction);
        })
        .WithName("CreateCashTransaction")
        .WithDescription("Creates a new cash transaction")
        .Produces<CashTransaction>(StatusCodes.Status201Created)
        .ProducesProblem(StatusCodes.Status400BadRequest);

        // Create Investment Transaction
        transactionEndpoints.MapPost("/investment", async (FinanceTackerDbContext context, CreateInvestmentTransactionRequestDto investmentTransactionDto) =>
        {
            if (investmentTransactionDto == null) return Results.BadRequest("Investment transaction data is required");

            var validationResults = new List<ValidationResult>();
            if (!Validator.TryValidateObject(investmentTransactionDto, new ValidationContext(investmentTransactionDto), validationResults, true))
            {
                return Results.BadRequest(validationResults);
            }

            // Create the investment transaction
            var investmentTransaction = new InvestmentTransaction
            {
                TransactionDate = investmentTransactionDto.TransactionDate,
                Description = investmentTransactionDto.Description,
                Quantity = investmentTransactionDto.Quantity,
                Price = investmentTransactionDto.Price,
                Fees = investmentTransactionDto.Fees,
                Commission = investmentTransactionDto.Commission,
                OrderType = investmentTransactionDto.OrderType,
                SecurityId = investmentTransactionDto.SecurityId,
                TransactionTypeId = investmentTransactionDto.TransactionTypeId,
                AccountPeriodId = investmentTransactionDto.AccountPeriodId
            };

            context.InvestmentTransactions.Add(investmentTransaction);
            await context.SaveChangesAsync();
            return Results.Created($"/api/v1/transactions/investment/{investmentTransaction.Id}", investmentTransaction);
        })
        .WithName("CreateInvestmentTransaction")
        .WithDescription("Creates a new investment transaction")
        .Produces<InvestmentTransaction>(StatusCodes.Status201Created)
        .ProducesProblem(StatusCodes.Status400BadRequest);
    }
}
