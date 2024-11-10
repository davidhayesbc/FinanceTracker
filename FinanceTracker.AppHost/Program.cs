using Projects;

var builder = DistributedApplication.CreateBuilder(args);
var password = builder.AddParameter("DatabaseServerPassword", true);

var sqlServer = builder.AddSqlServer("sqlServer", password, 1433)
    .WithDataBindMount("./SqlData")
    .WithLifetime(ContainerLifetime.Persistent);
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