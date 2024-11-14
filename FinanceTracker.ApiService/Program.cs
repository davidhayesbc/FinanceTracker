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

// Account Type Handlers
app.MapGet("/accountTypes", (FinanceTackerDbContext context) => { return context.AccountTypes.ToList(); });

app.MapPost("/accountTypes", async (FinanceTackerDbContext context, AccountType accountType) =>
{
    context.AccountTypes.Add(accountType);
    await context.SaveChangesAsync();
    return Results.Created($"/accountTypes/{accountType.Id}", accountType);
});

// Account Handlers
app.MapGet("/accounts", (FinanceTackerDbContext context) => { return context.Accounts.ToList(); });

app.MapGet("/accounts/{id}", async (FinanceTackerDbContext context, int id) =>
{
    var account = await context.Accounts.FindAsync(id);
    return account is not null ? Results.Ok(account) : Results.NotFound();
});
app.MapPost("/accounts", async (FinanceTackerDbContext context, Account account) =>
{
    context.Accounts.Add(account);
    await context.SaveChangesAsync();
    return Results.Created($"/accounts/{account.Id}", account);
});

// RecurringTransactions Handlers
app.MapGet("/recurringTransactions", (FinanceTackerDbContext context) => { return context.RecurringTransactions.ToList(); });

app.MapPost("/recurringTransactions", async (FinanceTackerDbContext context, RecurringTransaction recurringTransaction) =>
{
    context.RecurringTransactions.Add(recurringTransaction);
    await context.SaveChangesAsync();
    return Results.Created($"/recurringTransactions/{recurringTransaction.Id}", recurringTransaction);
});

// Transactions Handlers
app.MapGet("/transactions", (FinanceTackerDbContext context) => { return context.Transactions.ToList(); });

app.MapPost("/transactions", async (FinanceTackerDbContext context, Transaction transaction) =>
{
    context.Transactions.Add(transaction);
    await context.SaveChangesAsync();
    return Results.Created($"/transactions/{transaction.Id}", transaction);
});

// Transaction Split Handlers
app.MapGet("/transactionSplits", (FinanceTackerDbContext context) => { return context.TransactionSplits.ToList(); });

app.MapPost("/transactionSplits", async (FinanceTackerDbContext context, TransactionSplit transactionSplit) =>
{
    context.TransactionSplits.Add(transactionSplit);
    await context.SaveChangesAsync();
    return Results.Created($"/transactionSplits/{transactionSplit.Id}", transactionSplit);
});


// TransactionCategory Handlers
app.MapGet("/transactionCategories", (FinanceTackerDbContext context) => { return context.TransactionCategories.ToList(); });

app.MapPost("/transactionCategories", async (FinanceTackerDbContext context, TransactionCategory transactionCategory) =>
{
    context.TransactionCategories.Add(transactionCategory);
    await context.SaveChangesAsync();
    return Results.Created($"/transactionCategories/{transactionCategory.Id}", transactionCategory);
});

// TransactionType Handlers
app.MapGet("/transactionTypes", (FinanceTackerDbContext context) => { return context.TransactionTypes.ToList(); });

app.MapPost("/transactionTypes", async (FinanceTackerDbContext context, TransactionType transactionType) =>
{
    context.TransactionTypes.Add(transactionType);
    await context.SaveChangesAsync();
    return Results.Created($"/transactionTypes/{transactionType.Id}", transactionType);
});


app.MapDefaultEndpoints();

app.Run();
