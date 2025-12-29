using Microsoft.Extensions.Hosting;
using TraDe.Core;

namespace TraDe.Server;

public class MarketDataReplayService : BackgroundService
{
    private readonly OrderProcessingChannel _channel;
    private readonly ILogger<MarketDataReplayService> _logger;
    private readonly IHostEnvironment _env;

    public MarketDataReplayService(
        OrderProcessingChannel channel, 
        ILogger<MarketDataReplayService> logger,
        IHostEnvironment env)
    {
        _channel = channel;
        _logger = logger;
        _env = env;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        // Wait for the engine and persistence workers to start first
        await Task.Delay(2000, stoppingToken);

        string filePath = Path.Combine(_env.ContentRootPath, "market_data.csv");
        if (!File.Exists(filePath))
        {
            _logger.LogWarning("Market data file not found at {Path}", filePath);
            return;
        }

        _logger.LogInformation("Starting Market Data Replay...");

        using var reader = new StreamReader(filePath);
        await reader.ReadLineAsync(); // Skip header

        while (!reader.EndOfStream && !stoppingToken.IsCancellationRequested)
        {
            var line = await reader.ReadLineAsync();
            if (string.IsNullOrWhiteSpace(line)) continue;

            var parts = line.Split(',');
            if (parts.Length < 3) continue;

            if (decimal.TryParse(parts[0], out var price) &&
                decimal.TryParse(parts[1], out var qty) &&
                Enum.TryParse<OrderSide>(parts[2], out var side))
            {
                var order = new Order(price, qty, side);
                await _channel.TryAddOrderAsync(order, stoppingToken);
                
                // Simulate a slight delay between market orders
                await Task.Delay(100, stoppingToken); 
            }
        }

        _logger.LogInformation("Market Data Replay completed.");
    }
}