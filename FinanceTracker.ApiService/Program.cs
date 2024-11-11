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

app.MapGet("/weatherforecast", (FinanceTackerDbContext context) => { return context.AccountTypes.ToList(); });

app.MapGet("/accounts", (FinanceTackerDbContext context) => { return context.Accounts.ToList(); });

app.MapDefaultEndpoints();

app.Run();

internal record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}