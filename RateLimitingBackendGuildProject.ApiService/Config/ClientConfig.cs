namespace RateLimitingBackendGuildProject.ApiService.Config;

public class ClientConfig
{
    public string ClientId { get; set; } = string.Empty;
    public string ApiKey { get; set; } = string.Empty;
    public SubscriptionTier Subscription { get; set; }
}

public enum SubscriptionTier
{
    Free,
    Premium
}