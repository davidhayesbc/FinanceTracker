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

app.MapGet("/accountTypes", (FinanceTackerDbContext context) => { return context.AccountTypes.ToList(); });

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
app.MapDefaultEndpoints();

app.Run();
