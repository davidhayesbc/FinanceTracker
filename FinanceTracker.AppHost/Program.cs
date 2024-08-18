using Projects;

var builder = DistributedApplication.CreateBuilder(args);
var password = builder.AddParameter("DatabaseServerPassword", true);

var sqlServer = builder.AddSqlServer("sqlServer", password, 1433).WithDataVolume();
var database = sqlServer.AddDatabase("FinanceTracker");

var apiService = builder.AddProject<FinanceTracker_ApiService>("apiservice")
    .WithReference(database);

builder.AddProject<FinanceTracker_Web>("webfrontend")
    .WithExternalHttpEndpoints()
    .WithReference(apiService);

builder.AddProject<FinanceTracker_MigrationService>("migrationservice").WithReference(database);

builder.Build().Run();