using Projects;

var builder = DistributedApplication.CreateBuilder(args);

var sqlServer = builder.AddAzureSqlServer("sqlServer")
    .RunAsContainer(o =>
    {
        o.WithLifetime(ContainerLifetime.Persistent);
        o.WithDataBindMount("./SqlData");
    });

var database = sqlServer.AddDatabase("FinanceTracker");

var migrationService = builder.AddProject<FinanceTracker_MigrationService>("migrationservice")
    .WithReference(database)
    .WaitFor(sqlServer);

var apiService = builder.AddProject<FinanceTracker_ApiService>("apiservice")
    .WithReference(database)
    .WaitFor(sqlServer)
    .WaitForCompletion(migrationService);

builder.AddProject<FinanceTracker_Web>("webfrontend")
    .WithExternalHttpEndpoints()
    .WithReference(apiService).WaitFor(apiService);



builder.Build().Run();