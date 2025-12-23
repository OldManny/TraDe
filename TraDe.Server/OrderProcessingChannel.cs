using System.Threading.Channels;
using TraDe.Core;

namespace TraDe.Server;

public class OrderProcessingChannel
{
    private readonly Channel<Order> _channel;
    private const int MaxQueuedOrders = 10_000; // Safety limit

    public OrderProcessingChannel()
    {
        var options = new BoundedChannelOptions(MaxQueuedOrders)
        {
            SingleReader = true,
            SingleWriter = false,
            // The writer (API) will asynchronously wait, 
            // slowing down the ingestion rate.
            FullMode = BoundedChannelFullMode.Wait 
        };
        _channel = Channel.CreateBounded<Order>(options);
    }

    // Returns false if the channel is closed or cannot be written to.
    public async ValueTask<bool> TryAddOrderAsync(Order order, CancellationToken ct = default)
    {
        while (await _channel.Writer.WaitToWriteAsync(ct))
        {
            if (_channel.Writer.TryWrite(order)) return true;
        }
        return false;
    }

    public ChannelReader<Order> Reader => _channel.Reader;
    public void Complete() => _channel.Writer.TryComplete();
}