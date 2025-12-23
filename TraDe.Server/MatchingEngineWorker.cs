using System.Diagnostics;
using TraDe.Core;

namespace TraDe.Server;

public class MatchingEngineWorker(
    ILogger<MatchingEngineWorker> logger,
    OrderProcessingChannel processingChannel,
    OrderBook orderBook) : BackgroundService
{
    private readonly ILogger<MatchingEngineWorker> _logger = logger;
    private readonly OrderProcessingChannel _processingChannel = processingChannel;
    private readonly OrderBook _orderBook = orderBook;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Matching Engine Worker started.");

        // This handles the shutdown signal
        stoppingToken.Register(() => _processingChannel.Complete());

        // Stopwatching for performance benchmarking
        var sw = new Stopwatch();

        try
        {
            // When Complete() is called, this loop finishes naturally after the last item
            await foreach (var order in _processingChannel.Reader.ReadAllAsync(stoppingToken))
            {
                sw.Restart();
                
                var trades = _orderBook.AddOrder(order);

                sw.Stop();

                // Only log matches or slow processing
                if (trades.Count > 0)
                {
                    _logger.LogInformation("Processed Match in {Ticks} ticks. Trades generated: {Count}", 
                        sw.ElapsedTicks, trades.Count);
                }

                // Exit if the app is shutting down and the queue is empty.
                if (stoppingToken.IsCancellationRequested && _processingChannel.Reader.Count == 0)
                    break;
            }
        }
        catch (OperationCanceledException)
        {
            _logger.LogWarning("Matching Engine Worker forced to stop.");
        }
        finally
        {
            _logger.LogInformation("Matching Engine Worker shutdown complete. All orders drained.");
        }
    }
}