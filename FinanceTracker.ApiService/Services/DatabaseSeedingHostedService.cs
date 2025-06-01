using FinanceTracker.ApiService.Services;

namespace FinanceTracker.ApiService.Services;

/// <summary>
/// Hosted service that runs database seeding at startup when using in-memory database
/// This ensures that reference data is available for tests and development
/// </summary>
public class DatabaseSeedingHostedService : IHostedService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IConfiguration _configuration;
    private readonly ILogger<DatabaseSeedingHostedService> _logger;

    public DatabaseSeedingHostedService(
        IServiceProvider serviceProvider,
        IConfiguration configuration,
        ILogger<DatabaseSeedingHostedService> logger)
    {
        _serviceProvider = serviceProvider;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        // Only seed when using in-memory database (typically for tests)
        var useInMemoryDatabase = _configuration.GetValue<bool>("UseInMemoryDatabase");

        if (!useInMemoryDatabase)
        {
            _logger.LogInformation("Skipping database seeding because UseInMemoryDatabase is false");
            return;
        }

        _logger.LogInformation("UseInMemoryDatabase is true, starting database seeding...");

        using var scope = _serviceProvider.CreateScope();
        var seedingService = scope.ServiceProvider.GetRequiredService<DatabaseSeedingService>();

        await seedingService.SeedAsync(cancellationToken);

        _logger.LogInformation("Database seeding completed successfully");
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}
