using System.Diagnostics;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using TraDe.Core;

namespace TraDe.Server;

public class MatchingEngineWorker(
    ILogger<MatchingEngineWorker> logger,
    OrderProcessingChannel processingChannel,
    TradePersistenceChannel persistenceChannel,
    OrderBook orderBook) : BackgroundService
{
    private readonly ILogger<MatchingEngineWorker> _logger = logger;
    private readonly OrderProcessingChannel _processingChannel = processingChannel;
    private readonly TradePersistenceChannel _persistenceChannel = persistenceChannel;
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
                try
                {
                    sw.Restart();
                    var trades = _orderBook.AddOrder(order);
                    sw.Stop();

                    if (trades.Count > 0)
                    {
                        _logger.LogInformation("Match found in {Ticks} ticks. Processing persistence...", sw.ElapsedTicks);

                        foreach (var trade in trades)
                        {
                            // Hand off to the Persistence Layer asynchronously
                            await _persistenceChannel.AddTradeAsync(trade);
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error matching order {OrderId}", order.Id);
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