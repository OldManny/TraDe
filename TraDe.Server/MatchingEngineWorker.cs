using System.Diagnostics;
using TraDe.Core;
using Prometheus;

namespace TraDe.Server;

public class MatchingEngineWorker(
    ILogger<MatchingEngineWorker> logger,
    OrderProcessingChannel processingChannel,
    TradePersistenceChannel persistenceChannel,
    TradeNotificationChannel notificationChannel,
    OrderBook orderBook) : BackgroundService
{
    private readonly ILogger<MatchingEngineWorker> _logger = logger;
    private readonly OrderProcessingChannel _processingChannel = processingChannel;
    private readonly TradePersistenceChannel _persistenceChannel = persistenceChannel;
    private readonly TradeNotificationChannel _notificationChannel = notificationChannel;
    private readonly OrderBook _orderBook = orderBook;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Matching Engine Worker started.");

        // Signal completion to the channel when the service is stopping
        stoppingToken.Register(() => _processingChannel.Complete());

        var sw = new Stopwatch();

        try
        {
            // Read until the channel is empty and marked as complete
            await foreach (var order in _processingChannel.Reader.ReadAllAsync(stoppingToken))
            {
                // Measure Queue Depth (Snapshot before processing)
                TraDeMetrics.QueueDepth.Set(_processingChannel.Reader.Count);

                List<Core.Trade> trades;

                // Measure Latency (Histogram) & Execute Logic
                using (TraDeMetrics.MatchingDuration.NewTimer())
                {
                    sw.Restart(); 
                    trades = _orderBook.AddOrder(order);
                    sw.Stop();
                }

                // Measure Throughput (Counter)
                TraDeMetrics.OrdersProcessed.Inc();

                try
                {
                    if (trades.Count > 0)
                    {
                        //_logger.LogInformation("Match found in {Ticks} ticks. Processing persistence...", sw.ElapsedTicks);

                        foreach (var trade in trades)
                        {
                            // Hand off to Persistence
                            await _persistenceChannel.AddTradeAsync(trade);

                            // Hand off to UI (Fire and forget style)
                            await _notificationChannel.AddTradeAsync(trade);
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error processing trades for order {OrderId}", order.Id);
                }

                // Graceful drain: Check if it should exit after processing this order
                if (stoppingToken.IsCancellationRequested && _processingChannel.Reader.Count == 0)
                    break;
            }
        }
        catch (OperationCanceledException)
        {
            // Expected during normal shutdown
        }
        finally
        {
            _logger.LogInformation("Matching Engine Worker shutdown complete. All orders processed.");
        }
    }
}