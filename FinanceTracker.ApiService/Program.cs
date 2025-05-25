using FinanceTracker.Data.Models;
using FinanceTracker.ApiService.Dtos; 
using FinanceTracker.ApiService.Endpoints; // Add this line
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
apiV1.MapAccountTypeEndpoints();
apiV1.MapAccountEndpoints();
apiV1.MapRecurringTransactionEndpoints();
apiV1.MapTransactionEndpoints();
apiV1.MapTransactionSplitEndpoints();
apiV1.MapTransactionCategoryEndpoints();
apiV1.MapTransactionTypeEndpoints();

app.MapDefaultEndpoints();

app.Run();

// Make Program class public for integration tests
public partial class Program { }