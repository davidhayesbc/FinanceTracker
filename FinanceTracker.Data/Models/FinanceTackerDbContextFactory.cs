using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace FinanceTracker.Data.Models;

public class FinanceTackerDbContextFactory : IDesignTimeDbContextFactory<FinanceTackerDbContext>
{
    public FinanceTackerDbContext CreateDbContext(string[] args)
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json")
            .Build();

        var optionsBuilder = new DbContextOptionsBuilder<FinanceTackerDbContext>();
        var connectionString = configuration.GetConnectionString("FinanceTracker");
        optionsBuilder.UseSqlServer(connectionString);

        return new FinanceTackerDbContext(optionsBuilder.Options);
    }
}