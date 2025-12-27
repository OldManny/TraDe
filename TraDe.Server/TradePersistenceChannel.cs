using System.Threading.Channels;
using TraDe.Core;

namespace TraDe.Server;

// Decouples the Matching Engine from Database I/O
public class TradePersistenceChannel
{
    private readonly Channel<Trade> _channel = Channel.CreateUnbounded<Trade>();
    public ChannelReader<Trade> Reader => _channel.Reader;

    public async ValueTask AddTradeAsync(Trade trade) => await _channel.Writer.WriteAsync(trade);

    public void Complete() => _channel.Writer.TryComplete();
}