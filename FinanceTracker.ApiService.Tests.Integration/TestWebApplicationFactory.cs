using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using FinanceTracker.Data.Models;

namespace FinanceTracker.ApiService.Tests.Integration;

public class TestWebApplicationFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            ConfigureInMemoryDatabase(services);
        });
    }

    private static void ConfigureInMemoryDatabase(IServiceCollection services)
    {
        // Remove the real database service
        var descriptor = services.SingleOrDefault(
            d => d.ServiceType == typeof(DbContextOptions<FinanceTackerDbContext>));

        if (descriptor != null)
        {
            services.Remove(descriptor);
        }

        // Add in-memory database
        services.AddDbContext<FinanceTackerDbContext>(options =>
        {
            options.UseInMemoryDatabase("InMemoryDbForTesting");
        });

        // Build service provider to seed the database
        var serviceProvider = services.BuildServiceProvider();
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<FinanceTackerDbContext>();

        SeedDatabase(context);
    }

    private static void SeedDatabase(FinanceTackerDbContext context)
    {
        // Clear existing data
        context.Database.EnsureDeleted();
        context.Database.EnsureCreated();

        // Seed required data for tests
        var accountType = new AccountType
        {
            Id = 1,
            Type = "Checking"
        };

        var currency = new Currency
        {
            Id = 1,
            Name = "US Dollar",
            Symbol = "$",
            DisplaySymbol = "$"
        };

        context.AccountTypes.Add(accountType);
        context.Currencies.Add(currency);
        context.SaveChanges();
    }
}
