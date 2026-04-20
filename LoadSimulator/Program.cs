var apiBaseUrl = Environment.GetEnvironmentVariable("services__apiservice__https__0")
    ?? Environment.GetEnvironmentVariable("services__apiservice__http__0")
    ?? "http://localhost:5000";

var clients = new[]
{
    ("client-a", "key-client-a"),
    ("client-b", "key-client-b"),
    ("client-c", "key-client-c"),
};

var cts = new CancellationTokenSource();
Console.CancelKeyPress += (_, e) => { e.Cancel = true; cts.Cancel(); };

Console.WriteLine($"Starting load simulation against {apiBaseUrl}");
Console.WriteLine("Press Ctrl+C to stop\n");

var tasks = clients.Select(c => SimulateClient(c.Item1, c.Item2, apiBaseUrl, cts.Token));
await Task.WhenAll(tasks);

static async Task SimulateClient(string name, string apiKey, string baseUrl, CancellationToken ct)
{
    var http = new HttpClient { BaseAddress = new Uri(baseUrl) };
    http.DefaultRequestHeaders.Add("X-API-Key", apiKey);

    var timer = new PeriodicTimer(TimeSpan.FromMilliseconds(200)); // 5 requests/sec per client

    while (!ct.IsCancellationRequested)
    {
        try
        {
            await timer.WaitForNextTickAsync(ct);

            var response = await http.GetAsync("/weatherforecast", ct);

            var color = (int)response.StatusCode switch
            {
                200 => ConsoleColor.Green,
                429 => ConsoleColor.Red,
                401 => ConsoleColor.Yellow,
                _ => ConsoleColor.Gray
            };

            Console.ForegroundColor = color;
            Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] {name,-10} → {(int)response.StatusCode} {response.StatusCode}");
            Console.ResetColor();
        }
        catch (OperationCanceledException) { break; }
        catch (Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] {name,-10} → ERROR: {ex.Message}");
            Console.ResetColor();
        }
    }
}
