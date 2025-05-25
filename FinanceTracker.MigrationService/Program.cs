namespace FinanceTracker.MigrationService;

using FinanceTracker.Data.Models;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = Host.CreateApplicationBuilder(args);
        builder.AddServiceDefaults();
        builder.Services.AddHostedService<Worker>();

        // Configure OpenTelemetry
        builder.Services.AddOpenTelemetry()
            .WithTracing(tracing => tracing.AddSource(Worker.ActivitySourceName));

        // Add SQL Server DbContext
        builder.Services.AddDbContext<FinanceTackerDbContext>(options =>
            options.UseSqlServer(builder.Configuration.GetConnectionString("FinanceTracker")));

        var host = builder.Build();
        await host.RunAsync();
    }
}
