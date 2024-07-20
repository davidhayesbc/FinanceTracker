var builder = DistributedApplication.CreateBuilder(args);

var apiService = builder.AddProject<Projects.FinanceTracker_ApiService>("apiservice");

builder.AddProject<Projects.FinanceTracker_Web>("webfrontend")
    .WithExternalHttpEndpoints()
    .WithReference(apiService);

builder.Build().Run();
