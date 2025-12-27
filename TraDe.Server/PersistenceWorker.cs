using TraDe.Server.Data;

namespace TraDe.Server;

public class PersistenceWorker : BackgroundService
{
    private readonly ILogger<PersistenceWorker> _logger;
    private readonly TradePersistenceChannel _persistenceChannel;
    private readonly IServiceProvider _serviceProvider;
    private const int BatchSize = 100;

    public PersistenceWorker(
        ILogger<PersistenceWorker> logger,
        TradePersistenceChannel persistenceChannel,
        IServiceProvider serviceProvider)
    {
        _logger = logger;
        _persistenceChannel = persistenceChannel;
        _serviceProvider = serviceProvider;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Persistence Worker started.");
        var batch = new List<Core.Trade>();

        // Trigger batch saves every 5 seconds
        using var timer = new PeriodicTimer(TimeSpan.FromSeconds(5));

        while (!stoppingToken.IsCancellationRequested)
        {
            // Wait for a new trade or the 5-second timer
            while (_persistenceChannel.Reader.TryRead(out var trade))
            {
                batch.Add(trade);
                if (batch.Count >= BatchSize) break;
            }

            if (batch.Count > 0)
            {
                await SaveBatchAsync(batch);
                batch.Clear();
            }

            // Wait for the next cycle or a short delay to prevent CPU spinning
            await Task.Delay(1000, stoppingToken); 
        }
    }
    private async Task SaveBatchAsync(List<Core.Trade> trades)
    {
        if (trades.Count == 0) return;

        // Create a scope inside the singleton background worker
        using var scope = _serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<TradingDbContext>();

        try
        {
            await context.Trades.AddRangeAsync(trades);
            await context.SaveChangesAsync();
            _logger.LogInformation("Persisted batch of {Count} trades to PostgreSQL.", trades.Count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to persist trades to database.");
        }
    }
}