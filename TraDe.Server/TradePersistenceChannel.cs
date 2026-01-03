using System.Threading.Channels;
using TraDe.Core;

namespace TraDe.Server;

// Decouples the Matching Engine from Database I/O
public class TradePersistenceChannel
{
    // If DB is too slow, the Engine will wait (Backpressure) instead of crashing RAM.
    private readonly Channel<Trade> _channel = Channel.CreateBounded<Trade>(new BoundedChannelOptions(50_000)
    {
        FullMode = BoundedChannelFullMode.Wait,
        SingleReader = true,
        SingleWriter = true
    });
    public ChannelReader<Trade> Reader => _channel.Reader;

    public async ValueTask AddTradeAsync(Trade trade) => await _channel.Writer.WriteAsync(trade);

    public void Complete() => _channel.Writer.TryComplete();
}