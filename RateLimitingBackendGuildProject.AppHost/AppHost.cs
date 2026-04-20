var builder = DistributedApplication.CreateBuilder(args);

var apiService = builder.AddProject<Projects.RateLimitingBackendGuildProject_ApiService>("apiservice")
    .WithHttpHealthCheck("/health")
    .WithReplicas(2);

builder.AddProject<Projects.LoadSimulator>("loadsimulator")
    .WithReference(apiService)
    .WaitFor(apiService);

builder.Build().Run();