using RateLimitingBackendGuildProject.ApiService.Config;

namespace RateLimitingBackendGuildProject.ApiService.Middleware;

public class ApiKeyMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context, ClientStore clientStore)
    {
        if (context.Request.Path.StartsWithSegments("/health") ||
            context.Request.Path.StartsWithSegments("/alive"))
        {
            await next(context);
            return;
        }

        var apiKey = context.Request.Headers["X-API-Key"].FirstOrDefault();

        if (apiKey is null || !clientStore.TryGetClient(apiKey, out var client))
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await context.Response.WriteAsync("Invalid or missing API key.");
            return;
        }

        context.Items["ClientId"] = client!.ClientId;
        await next(context);
    }
}
