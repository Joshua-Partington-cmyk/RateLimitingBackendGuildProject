namespace RateLimitingBackendGuildProject.ApiService.Config;

public class ClientStore
{
    private readonly Dictionary<string, ClientConfig> _clients;

    public ClientStore(IConfiguration configuration)
    {
        _clients = configuration
            .GetSection("Clients")
            .Get<List<ClientConfig>>()!
            .ToDictionary(c => c.ApiKey);
    }

    public bool TryGetClient(string apiKey, out ClientConfig? client)
        => _clients.TryGetValue(apiKey, out client);
}
