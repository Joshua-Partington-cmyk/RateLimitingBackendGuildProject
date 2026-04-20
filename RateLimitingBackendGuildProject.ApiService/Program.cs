using RateLimitingBackendGuildProject.ApiService.Endpoints;
using RateLimitingBackendGuildProject.ApiService.Middleware;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();
builder.Services.AddProblemDetails();
builder.Services.AddOpenApi();

var app = builder.Build();

app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}
app.UseMiddleware<ApiKeyMiddleware>();

app.MapWeatherEndpoints();
app.MapDefaultEndpoints();

app.Run();