using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using FinanceTracker.Data.Models;

namespace FinanceTracker.ApiService.Tests.Integration;

/// <summary>
/// Custom WebApplicationFactory for testing the FinanceTracker API with an in-memory database
/// </summary>
public class FinanceTrackerWebApplicationFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            // Remove the existing DbContext registration
            services.RemoveAll(typeof(DbContextOptions<FinanceTackerDbContext>));
            services.RemoveAll(typeof(FinanceTackerDbContext));

            // Add an in-memory database for testing
            services.AddDbContext<FinanceTackerDbContext>(options =>
            {
                options.UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString());
            });

            // Build the service provider and ensure the database is created
            var serviceProvider = services.BuildServiceProvider();
            using var scope = serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<FinanceTackerDbContext>();
            context.Database.EnsureCreated();
        });

        builder.UseEnvironment("Testing");
    }

    /// <summary>
    /// Get a fresh DbContext for the test database
    /// </summary>
    public FinanceTackerDbContext GetDbContext()
    {
        var scope = Services.CreateScope();
        return scope.ServiceProvider.GetRequiredService<FinanceTackerDbContext>();
    }
}
