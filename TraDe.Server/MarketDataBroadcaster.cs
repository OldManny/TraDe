using Microsoft.AspNetCore.SignalR;
using TraDe.Server.Hubs;
using TraDe.Core;

namespace TraDe.Server;

public class MarketDataBroadcaster : BackgroundService
{
    private readonly TradeNotificationChannel _channel;
    private readonly IHubContext<MarketDataHub> _hub;
    private readonly ILogger<MarketDataBroadcaster> _logger;

    // Buffer: Send updates every 50ms to save bandwidth
    private const int BroadcastIntervalMs = 50; 

    public MarketDataBroadcaster(
        TradeNotificationChannel channel, 
        IHubContext<MarketDataHub> hub,
        ILogger<MarketDataBroadcaster> logger)
    {
        _channel = channel;
        _hub = hub;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var buffer = new List<Trade>();
        using var timer = new PeriodicTimer(TimeSpan.FromMilliseconds(BroadcastIntervalMs));

        while (await timer.WaitForNextTickAsync(stoppingToken))
        {
            // Drain the channel buffer for this tick
            while (_channel.Reader.TryRead(out var trade))
            {
                buffer.Add(trade);
                if (buffer.Count >= 100) break; // Cap batch size
            }

            if (buffer.Count > 0)
            {
                // Send to UI
                await _hub.Clients.All.SendAsync("ReceiveTrades", buffer, stoppingToken);
                buffer.Clear();
            }
        }
    }
}