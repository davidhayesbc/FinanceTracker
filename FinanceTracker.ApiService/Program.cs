using FinanceTracker.Data.Models;
using FinanceTracker.ApiService.Dtos; // DTO नेमस्पेस जोड़ें
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using System.ComponentModel.DataAnnotations;

var builder = WebApplication.CreateBuilder(args);

// Define a CORS policy name
const string WebAppCorsPolicy = "WebAppCorsPolicy";

// Add service defaults & Aspire components.
builder.AddServiceDefaults();

// Add CORS services
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: WebAppCorsPolicy,
                      policyBuilder =>
                      {
                          // In development, allow the Vite dev server's origin.
                          // For production, you'll want to configure this more restrictively
                          // based on your frontend's actual deployed URL.
                          var frontendUrl = builder.Configuration["services:web:https:0:url"] ?? builder.Configuration["services:web:http:0:url"];
                          if (string.IsNullOrEmpty(frontendUrl) && builder.Environment.IsDevelopment())
                          {
                              frontendUrl = "http://localhost:5174"; // Default Vite port
                          }

                          if (!string.IsNullOrEmpty(frontendUrl))
                          {
                              policyBuilder.WithOrigins(frontendUrl)
                                  .AllowAnyHeader()
                                  .AllowAnyMethod();
                          }
                          else if (builder.Environment.IsDevelopment())
                          {
                              // Fallback for local dev if Aspire service discovery isn't providing the URL
                              // or if running ApiService standalone.
                              policyBuilder.WithOrigins("http://localhost:5174")
                                   .AllowAnyHeader()
                                   .AllowAnyMethod();
                          }
                          // else in production, you might want to throw an error if the URL isn't configured
                          // or have a more secure default.
                      });
});

builder.AddSqlServerDbContext<FinanceTackerDbContext>("FinanceTracker");

builder.Services.AddHealthChecks()
    .AddDbContextCheck<FinanceTackerDbContext>("FinanceTrackerDbContext");

// Add Swagger services with enhanced configuration
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Finance Tracker API",
        Version = "v1",
        Description = "API for tracking financial transactions and accounts",
        Contact = new OpenApiContact
        {
            Name = "Finance Tracker Team"
        }
    });

    // Add API grouping tags
    c.TagActionsBy(api => new[] { api.GroupName });
    c.DocInclusionPredicate((name, api) => true);
});

var app = builder.Build();

// Configure the HTTP request pipeline.

// Use CORS middleware - place it early in the pipeline
app.UseCors(WebAppCorsPolicy);

app.UseHttpsRedirection();
app.UseExceptionHandler("/error"); // Redirects to an error-handling endpoint

// Enable Swagger and Swagger UI
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Finance Tracker API v1");
        c.DocumentTitle = "Finance Tracker API Documentation";
        c.DefaultModelsExpandDepth(-1); // Hide schemas section by default
    });
}

// Apply versioned API prefix
var apiV1 = app.MapGroup("/api/v1");

//Map the Api endpoints
MapAccountTypeEndPoints(apiV1);
MapAccountEndpoints(apiV1);
MapRecurringTransactionEndpoints(apiV1);
MapTransactionEndpoints(apiV1);
MapTransactionSplitEndpoints(apiV1);
MapTransactionCategoryEndPoints(apiV1);
MapTransactionTypeEndPoints(apiV1);

app.MapDefaultEndpoints();

app.Run();
return;

void MapAccountTypeEndPoints(RouteGroupBuilder group)
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

void MapAccountEndpoints(RouteGroupBuilder group)
{
    var accountEndpoints = group.MapGroup("/accounts")
        .WithTags("Accounts")
        .WithGroupName("Accounts");

    accountEndpoints.MapGet("/", async (FinanceTackerDbContext context) =>
    {
        var accounts = await context.Accounts
            .Include(a => a.AccountType)
            .Include(a => a.Currency)
            .Select(a => new AccountDto
            {
                Id = a.Id,
                Name = a.Name,
                OpeningBalance = a.OpeningBalance,
                AccountTypeName = a.AccountType != null ? a.AccountType.Type : string.Empty,
                CurrencySymbol = a.Currency != null ? a.Currency.Symbol : string.Empty,
                CurrencyDisplaySymbol = a.Currency != null ? a.Currency.DisplaySymbol : string.Empty,
                OpenedDate = a.OpenDate
            })
            .ToListAsync();
        return Results.Ok(accounts);
    })
    .WithName("GetAllAccounts")
    .WithDescription("Gets all accounts with flattened related data")
    .Produces<List<AccountDto>>(StatusCodes.Status200OK);

    accountEndpoints.MapGet("/{id}", async (FinanceTackerDbContext context, int id) =>
    {
        var account = await context.Accounts.FindAsync(id);
        return account is not null ?
            Results.Ok(account) :
            Results.NotFound($"Account with ID {id} not found");
    })
    .WithName("GetAccountById")
    .WithDescription("Gets an account by its ID")
    .Produces<Account>(StatusCodes.Status200OK)
    .ProducesProblem(StatusCodes.Status404NotFound);

    accountEndpoints.MapGet("/{id}/transactions", async (FinanceTackerDbContext context, int id) =>
    {
        var accountExists = await context.Accounts.AnyAsync(a => a.Id == id);
        if (!accountExists)
            return Results.NotFound($"Account with ID {id} not found");

        var transactions = await context.Transactions.Where(t => t.AccountId == id).ToListAsync();
        return Results.Ok(transactions);
    })
    .WithName("GetAccountTransactions")
    .WithDescription("Gets all transactions for a specific account")
    .Produces<List<Transaction>>(StatusCodes.Status200OK)
    .ProducesProblem(StatusCodes.Status404NotFound);

    accountEndpoints.MapGet("/{id}/recurringTransactions", async (FinanceTackerDbContext context, int id) =>
    {
        var accountExists = await context.Accounts.AnyAsync(a => a.Id == id);
        if (!accountExists)
            return Results.NotFound($"Account with ID {id} not found");

        var recurringTransactions = await context.RecurringTransactions.Where(t => t.AccountId == id).ToListAsync();
        return Results.Ok(recurringTransactions);
    })
    .WithName("GetAccountRecurringTransactions")
    .WithDescription("Gets all recurring transactions for a specific account")
    .Produces<List<RecurringTransaction>>(StatusCodes.Status200OK)
    .ProducesProblem(StatusCodes.Status404NotFound);

    accountEndpoints.MapPost("/", async (FinanceTackerDbContext context, Account account) =>
    {
        if (account == null) return Results.BadRequest("Account data is required");

        var validationResults = new List<ValidationResult>();
        if (!Validator.TryValidateObject(account, new ValidationContext(account), validationResults, true))
        {
            return Results.BadRequest(validationResults);
        }

        context.Accounts.Add(account);
        await context.SaveChangesAsync();
        return Results.Created($"/api/v1/accounts/{account.Id}", account);
    })
    .WithName("CreateAccount")
    .WithDescription("Creates a new account")
    .Produces<Account>(StatusCodes.Status201Created)
    .ProducesProblem(StatusCodes.Status400BadRequest);
}

void MapRecurringTransactionEndpoints(RouteGroupBuilder group)
{
    var recurringTransactionsEndpoints = group.MapGroup("/recurringTransactions")
        .WithTags("Recurring Transactions")
        .WithGroupName("Recurring Transactions");

    recurringTransactionsEndpoints.MapGet("/", async (FinanceTackerDbContext context) =>
    {
        var recurringTransactions = await context.RecurringTransactions.ToListAsync();
        return Results.Ok(recurringTransactions);
    })
    .WithName("GetAllRecurringTransactions")
    .WithDescription("Gets all recurring transactions")
    .Produces<List<RecurringTransaction>>(StatusCodes.Status200OK);

    recurringTransactionsEndpoints.MapPost("/", async (FinanceTackerDbContext context, RecurringTransaction recurringTransaction) =>
    {
        if (recurringTransaction == null) return Results.BadRequest("Recurring transaction data is required");

        var validationResults = new List<ValidationResult>();
        if (!Validator.TryValidateObject(recurringTransaction, new ValidationContext(recurringTransaction), validationResults, true))
        {
            return Results.BadRequest(validationResults);
        }

        // Validate that the account exists
        if (!await context.Accounts.AnyAsync(a => a.Id == recurringTransaction.AccountId))
        {
            return Results.BadRequest($"Account with ID {recurringTransaction.AccountId} does not exist");
        }

        context.RecurringTransactions.Add(recurringTransaction);
        await context.SaveChangesAsync();
        return Results.Created($"/api/v1/recurringTransactions/{recurringTransaction.Id}", recurringTransaction);
    })
    .WithName("CreateRecurringTransaction")
    .WithDescription("Creates a new recurring transaction")
    .Produces<RecurringTransaction>(StatusCodes.Status201Created)
    .ProducesProblem(StatusCodes.Status400BadRequest);
}

void MapTransactionEndpoints(RouteGroupBuilder group)
{
    var transactionEndpoints = group.MapGroup("/transactions")
        .WithTags("Transactions")
        .WithGroupName("Transactions");

    transactionEndpoints.MapGet("/", async (FinanceTackerDbContext context) =>
    {
        var transactions = await context.Transactions.ToListAsync();
        return Results.Ok(transactions);
    })
    .WithName("GetAllTransactions")
    .WithDescription("Gets all transactions")
    .Produces<List<Transaction>>(StatusCodes.Status200OK);

    transactionEndpoints.MapGet("/{id}", async (FinanceTackerDbContext context, int id) =>
    {
        var transaction = await context.Transactions.FindAsync(id);
        return transaction is not null ?
            Results.Ok(transaction) :
            Results.NotFound($"Transaction with ID {id} not found");
    })
    .WithName("GetTransactionById")
    .WithDescription("Gets a transaction by its ID")
    .Produces<Transaction>(StatusCodes.Status200OK)
    .ProducesProblem(StatusCodes.Status404NotFound);

    transactionEndpoints.MapGet("/{id}/transactionSplits", async (FinanceTackerDbContext context, int id) =>
    {
        var transactionExists = await context.Transactions.AnyAsync(t => t.Id == id);
        if (!transactionExists)
            return Results.NotFound($"Transaction with ID {id} not found");

        var transactionSplits = await context.TransactionSplits.Where(t => t.TransactionId == id).ToListAsync();
        return Results.Ok(transactionSplits);
    })
    .WithName("GetTransactionSplits")
    .WithDescription("Gets all splits for a specific transaction")
    .Produces<List<TransactionSplit>>(StatusCodes.Status200OK)
    .ProducesProblem(StatusCodes.Status404NotFound);

    transactionEndpoints.MapPost("/", async (FinanceTackerDbContext context, Transaction transaction) =>
    {
        if (transaction == null) return Results.BadRequest("Transaction data is required");

        var validationResults = new List<ValidationResult>();
        if (!Validator.TryValidateObject(transaction, new ValidationContext(transaction), validationResults, true))
        {
            return Results.BadRequest(validationResults);
        }

        // Validate that the account exists
        if (!await context.Accounts.AnyAsync(a => a.Id == transaction.AccountId))
        {
            return Results.BadRequest($"Account with ID {transaction.AccountId} does not exist");
        }

        context.Transactions.Add(transaction);
        await context.SaveChangesAsync();
        return Results.Created($"/api/v1/transactions/{transaction.Id}", transaction);
    })
    .WithName("CreateTransaction")
    .WithDescription("Creates a new transaction")
    .Produces<Transaction>(StatusCodes.Status201Created)
    .ProducesProblem(StatusCodes.Status400BadRequest);
}

void MapTransactionSplitEndpoints(RouteGroupBuilder group)
{
    var transactionSplitEndpoints = group.MapGroup("/transactionSplits")
        .WithTags("Transaction Splits")
        .WithGroupName("Transaction Splits");

    transactionSplitEndpoints.MapGet("/", async (FinanceTackerDbContext context) =>
    {
        var transactionSplits = await context.TransactionSplits.ToListAsync();
        return Results.Ok(transactionSplits);
    })
    .WithName("GetAllTransactionSplits")
    .WithDescription("Gets all transaction splits")
    .Produces<List<TransactionSplit>>(StatusCodes.Status200OK);

    transactionSplitEndpoints.MapPost("/", async (FinanceTackerDbContext context, TransactionSplit transactionSplit) =>
    {
        if (transactionSplit == null) return Results.BadRequest("Transaction split data is required");

        var validationResults = new List<ValidationResult>();
        if (!Validator.TryValidateObject(transactionSplit, new ValidationContext(transactionSplit), validationResults, true))
        {
            return Results.BadRequest(validationResults);
        }

        // Validate that the transaction exists
        if (!await context.Transactions.AnyAsync(t => t.Id == transactionSplit.TransactionId))
        {
            return Results.BadRequest($"Transaction with ID {transactionSplit.TransactionId} does not exist");
        }

        context.TransactionSplits.Add(transactionSplit);
        await context.SaveChangesAsync();
        return Results.Created($"/api/v1/transactionSplits/{transactionSplit.Id}", transactionSplit);
    })
    .WithName("CreateTransactionSplit")
    .WithDescription("Creates a new transaction split")
    .Produces<TransactionSplit>(StatusCodes.Status201Created)
    .ProducesProblem(StatusCodes.Status400BadRequest);
}

void MapTransactionCategoryEndPoints(RouteGroupBuilder group)
{
    var transactionCategoryEndpoints = group.MapGroup("/transactionCategories")
        .WithTags("Transaction Categories")
        .WithGroupName("Transaction Categories");

    transactionCategoryEndpoints.MapGet("/", async (FinanceTackerDbContext context) =>
    {
        var transactionCategories = await context.TransactionCategories.ToListAsync();
        return Results.Ok(transactionCategories);
    })
    .WithName("GetAllTransactionCategories")
    .WithDescription("Gets all transaction categories")
    .Produces<List<TransactionCategory>>(StatusCodes.Status200OK);

    transactionCategoryEndpoints.MapPost("/", async (FinanceTackerDbContext context, TransactionCategory transactionCategory) =>
    {
        if (transactionCategory == null) return Results.BadRequest("Transaction category data is required");

        var validationResults = new List<ValidationResult>();
        if (!Validator.TryValidateObject(transactionCategory, new ValidationContext(transactionCategory), validationResults, true))
        {
            return Results.BadRequest(validationResults);
        }

        context.TransactionCategories.Add(transactionCategory);
        await context.SaveChangesAsync();
        return Results.Created($"/api/v1/transactionCategories/{transactionCategory.Id}", transactionCategory);
    })
    .WithName("CreateTransactionCategory")
    .WithDescription("Creates a new transaction category")
    .Produces<TransactionCategory>(StatusCodes.Status201Created)
    .ProducesProblem(StatusCodes.Status400BadRequest);
}

void MapTransactionTypeEndPoints(RouteGroupBuilder group)
{
    var transactionTypeEndpoints = group.MapGroup("/transactionTypes")
        .WithTags("Transaction Types")
        .WithGroupName("Transaction Types");

    transactionTypeEndpoints.MapGet("/", async (FinanceTackerDbContext context) =>
    {
        var transactionTypes = await context.TransactionTypes.ToListAsync();
        return Results.Ok(transactionTypes);
    })
    .WithName("GetAllTransactionTypes")
    .WithDescription("Gets all transaction types")
    .Produces<List<TransactionType>>(StatusCodes.Status200OK);

    transactionTypeEndpoints.MapPost("/", async (FinanceTackerDbContext context, TransactionType transactionType) =>
    {
        if (transactionType == null) return Results.BadRequest("Transaction type data is required");

        var validationResults = new List<ValidationResult>();
        if (!Validator.TryValidateObject(transactionType, new ValidationContext(transactionType), validationResults, true))
        {
            return Results.BadRequest(validationResults);
        }

        context.TransactionTypes.Add(transactionType);
        await context.SaveChangesAsync();
        return Results.Created($"/api/v1/transactionTypes/{transactionType.Id}", transactionType);
    })
    .WithName("CreateTransactionType")
    .WithDescription("Creates a new transaction type")
    .Produces<TransactionType>(StatusCodes.Status201Created)
    .ProducesProblem(StatusCodes.Status400BadRequest);
}