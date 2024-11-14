using FinanceTracker.Data.Models;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add service defaults & Aspire components.
builder.AddServiceDefaults();
// Add services to the container.
builder.Services.AddProblemDetails();

builder.Services.AddDbContext<FinanceTackerDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("FinanceTracker")));

builder.Services.AddHealthChecks()
    .AddDbContextCheck<FinanceTackerDbContext>("FinanceTrackerDbContext");

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseExceptionHandler();

MapAccountTypeEndPoints();
MapAccountEndpoints();
MapRecurringTransactionEndpoints();
MapTransactionEndpoints();
MapTransactionSplitEndpoints();
MapTransactionCategoryEndPoints();
MapTransactionTypeEndPoints();

app.MapDefaultEndpoints();

app.Run();

void MapAccountTypeEndPoints()
{
    // Account Type Handlers
    app.MapGet("/accountTypes", (FinanceTackerDbContext context) => { return context.AccountTypes.ToList(); });

    app.MapPost("/accountTypes", async (FinanceTackerDbContext context, AccountType accountType) =>
    {
        context.AccountTypes.Add(accountType);
        await context.SaveChangesAsync();
        return Results.Created($"/accountTypes/{accountType.Id}", accountType);
    });
}

void MapAccountEndpoints()
{
    // Account Handlers
    app.MapGet("/accounts", (FinanceTackerDbContext context) => { return context.Accounts.ToList(); });
    app.MapGet("/accounts/{id}", async (FinanceTackerDbContext context, int id) =>
    {
        var account = await context.Accounts.FindAsync(id);
        return account is not null ? Results.Ok(account) : Results.NotFound();
    });
    app.MapGet("/accounts/{id}/transactions", async (FinanceTackerDbContext context, int id) =>
    {
        var transactions = context.Transactions.Where(t => t.AccountId == id);
        return Results.Ok(transactions);
    });
    app.MapGet("/accounts/{id}/recurringTransactions", async (FinanceTackerDbContext context, int id) =>
    {
        var recurringTransactions = context.RecurringTransactions.Where(t => t.AccountId == id);
        return Results.Ok(recurringTransactions);
    });
    app.MapPost("/accounts", async (FinanceTackerDbContext context, Account account) =>
    {
        context.Accounts.Add(account);
        await context.SaveChangesAsync();
        return Results.Created($"/accounts/{account.Id}", account);
    });
}

void MapRecurringTransactionEndpoints()
{
    // RecurringTransactions Handlers
    app.MapGet("/recurringTransactions", (FinanceTackerDbContext context) => { return context.RecurringTransactions.ToList(); });
    app.MapPost("/recurringTransactions", async (FinanceTackerDbContext context, RecurringTransaction recurringTransaction) =>
    {
        context.RecurringTransactions.Add(recurringTransaction);
        await context.SaveChangesAsync();
        return Results.Created($"/recurringTransactions/{recurringTransaction.Id}", recurringTransaction);
    });
}

void MapTransactionEndpoints()
{
    // Transactions Handlers
    app.MapGet("/transactions", (FinanceTackerDbContext context) => { return context.Transactions.ToList(); });
    app.MapGet("/transactions/{id}", async (FinanceTackerDbContext context, int id) =>
    {
        var transaction = await context.Transactions.FindAsync(id);
        return transaction is not null ? Results.Ok(transaction) : Results.NotFound();
    });
    app.MapGet("/transactions/{id}/transactionSplits", async (FinanceTackerDbContext context, int id) =>
    {
        var transactionSplits =  context.TransactionSplits.Where(t => t.TransactionId == id);
        return Results.Ok(transactionSplits);
    });
    app.MapPost("/transactions", async (FinanceTackerDbContext context, Transaction transaction) =>
    {
        context.Transactions.Add(transaction);
        await context.SaveChangesAsync();
        return Results.Created($"/transactions/{transaction.Id}", transaction);
    });
}

void MapTransactionSplitEndpoints()
{
    // Transaction Split Handlers
    app.MapGet("/transactionSplits", (FinanceTackerDbContext context) => { return context.TransactionSplits.ToList(); });
    app.MapPost("/transactionSplits", async (FinanceTackerDbContext context, TransactionSplit transactionSplit) =>
    {
        context.TransactionSplits.Add(transactionSplit);
        await context.SaveChangesAsync();
        return Results.Created($"/transactionSplits/{transactionSplit.Id}", transactionSplit);
    });
}

void MapTransactionCategoryEndPoints()
{
    // TransactionCategory Handlers
    app.MapGet("/transactionCategories", (FinanceTackerDbContext context) => { return context.TransactionCategories.ToList(); });
    app.MapPost("/transactionCategories", async (FinanceTackerDbContext context, TransactionCategory transactionCategory) =>
    {
        context.TransactionCategories.Add(transactionCategory);
        await context.SaveChangesAsync();
        return Results.Created($"/transactionCategories/{transactionCategory.Id}", transactionCategory);
    });
}

void MapTransactionTypeEndPoints()
{
    // TransactionType Handlers
    app.MapGet("/transactionTypes", (FinanceTackerDbContext context) => { return context.TransactionTypes.ToList(); });
    app.MapPost("/transactionTypes", async (FinanceTackerDbContext context, TransactionType transactionType) =>
    {
        context.TransactionTypes.Add(transactionType);
        await context.SaveChangesAsync();
        return Results.Created($"/transactionTypes/{transactionType.Id}", transactionType);
    });
}