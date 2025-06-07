using Projects;

var builder = DistributedApplication.CreateBuilder(args);

// Use different data mount for tests vs development to keep data isolated
var isTestEnvironment = builder.Environment.EnvironmentName == "Testing" ||
                        Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Testing" ||
                        Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER") == "true";

var sqlDataPath = isTestEnvironment ? "./SqlData_Tests" : "./SqlData";

var sqlServer = builder.AddAzureSqlServer("sqlServer")
    .RunAsContainer(o =>
    {
        o.WithLifetime(ContainerLifetime.Persistent);
        o.WithDataBindMount(sqlDataPath); //Use 127.0.0.1 to connect with SSMS
        o.WithImagePullPolicy(ImagePullPolicy.Always);
        o.WithEndpoint(targetPort: 1433, port: 1433, name: "sql"); // Expose SQL Server port to host for SSMS access
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
    .WithExternalHttpEndpoints();

builder.Build().Run();