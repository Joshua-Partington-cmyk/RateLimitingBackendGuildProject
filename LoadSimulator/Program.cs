var apiBaseUrl = Environment.GetEnvironmentVariable("services__apiservice__https__0")
    ?? Environment.GetEnvironmentVariable("services__apiservice__http__0")
    ?? "http://localhost:5000";

// Set to true to enable burst simulation
const bool burstEnabled = true;
const int burstSize = 30;                           // requests fired instantly per burst
const int burstIntervalSeconds = 30;                // how often a burst occurs

var clients = new[]
{
    ("client-a", "key-client-a"),
    ("client-b", "key-client-b"),
    ("client-c", "key-client-c"),
};

var cts = new CancellationTokenSource();
Console.CancelKeyPress += (_, e) => { e.Cancel = true; cts.Cancel(); };

Console.WriteLine($"Starting load simulation against {apiBaseUrl}");
Console.WriteLine($"Burst mode: {(burstEnabled ? $"enabled (size: {burstSize}, every {burstIntervalSeconds}s)" : "disabled")}\n");

var tasks = clients.Select(c => SimulateClient(c.Item1, c.Item2, apiBaseUrl, cts.Token));
await Task.WhenAll(tasks);

static async Task SimulateClient(string name, string apiKey, string baseUrl, CancellationToken ct)
{
    var http = new HttpClient { BaseAddress = new Uri(baseUrl) };
    http.DefaultRequestHeaders.Add("X-API-Key", apiKey);

    var steadyTimer = new PeriodicTimer(TimeSpan.FromMilliseconds(200)); // 5 req/sec steady
    var lastBurst = DateTime.UtcNow;

    while (!ct.IsCancellationRequested)
    {
        try
        {
            await steadyTimer.WaitForNextTickAsync(ct);

            // Fire burst if enabled and interval has elapsed
            if (burstEnabled && (DateTime.UtcNow - lastBurst).TotalSeconds >= burstIntervalSeconds)
            {
                lastBurst = DateTime.UtcNow;
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] {name,-10} → BURST ({burstSize} requests)");
                Console.ResetColor();

                var burstTasks = Enumerable.Range(0, burstSize)
                    .Select(_ => SendAndLog(http, name, ct));
                await Task.WhenAll(burstTasks);
            }
            else
            {
                await SendAndLog(http, name, ct);
            }
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

static async Task SendAndLog(HttpClient http, string name, CancellationToken ct)
{
    var response = await http.GetAsync("/weatherforecast", ct);

    var color = (int)response.StatusCode switch
    {
        200 => ConsoleColor.Green,
        429 => ConsoleColor.Red,
        401 => ConsoleColor.Yellow,
        _   => ConsoleColor.Gray
    };

    Console.ForegroundColor = color;
    Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] {name,-10} → {(int)response.StatusCode} {response.StatusCode}");
    Console.ResetColor();
}
