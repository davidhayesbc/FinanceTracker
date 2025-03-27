using Projects;

var builder = DistributedApplication.CreateBuilder(args);

var sqlServer = builder.AddAzureSqlServer("sqlServer")
    .RunAsContainer(o =>
    {
        o.WithLifetime(ContainerLifetime.Persistent);
        o.WithDataBindMount("./SqlData");
        o.WithEndpoint(port: 1433, targetPort: 1433, name: "sql", scheme: "tcp");
    });


var database = sqlServer.AddDatabase("FinanceTracker");

var migrationService = builder.AddProject<FinanceTracker_MigrationService>("migrationservice")
    .WithReference(database)
    .WaitFor(sqlServer);

var apiService = builder.AddProject<FinanceTracker_ApiService>("apiservice")
    .WithReference(database)
    .WithHttpHealthCheck("/health")
    .WaitFor(sqlServer)
    .WaitForCompletion(migrationService);

var webApp = builder.AddNpmApp("web", "../FinanceTracker.Web", "dev")
    .WithReference(apiService)
    .WaitForCompletion(migrationService)
    .WithEndpoint(port: 80, targetPort: 5173, name: "web-http", scheme: "http")
    .WithEndpoint(port: 443, targetPort: 5173, name: "web-https", scheme: "https");

builder.Build().Run();