namespace RateLimitingBackendGuildProject.ApiService.Middleware;

public class ApiKeyMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context)
    {
        if (context.Request.Path.StartsWithSegments("/health") ||
            context.Request.Path.StartsWithSegments("/alive"))
        {
            await next(context);
            return;
        }

        if (!context.Request.Headers.ContainsKey("X-API-Key"))
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await context.Response.WriteAsync("Missing API key.");
            return;
        }

        await next(context);
    }
}
