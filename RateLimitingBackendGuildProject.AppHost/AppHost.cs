var builder = DistributedApplication.CreateBuilder(args);

//var redis = builder.AddRedis("redis");

var apiService = builder.AddProject<Projects.RateLimitingBackendGuildProject_ApiService>("apiservice")
    .WithHttpHealthCheck("/health")
    .WithReplicas(2);
    //.WithReference(redis);

builder.AddProject<Projects.LoadSimulator>("loadsimulator")
    .WithReference(apiService)
    .WaitFor(apiService);

builder.Build().Run();
