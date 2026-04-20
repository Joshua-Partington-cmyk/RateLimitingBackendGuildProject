var builder = DistributedApplication.CreateBuilder(args);

var apiService = builder.AddProject<Projects.RateLimitingBackendGuildProject_ApiService>("apiservice")
    .WithHttpHealthCheck("/health");

builder.Build().Run();