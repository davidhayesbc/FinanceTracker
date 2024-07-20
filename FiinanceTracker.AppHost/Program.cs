var builder = DistributedApplication.CreateBuilder(args);

var apiService = builder.AddProject<Projects.FiinanceTracker_ApiService>("apiservice");

builder.AddProject<Projects.FiinanceTracker_Web>("webfrontend")
    .WithExternalHttpEndpoints()
    .WithReference(apiService);

builder.Build().Run();
