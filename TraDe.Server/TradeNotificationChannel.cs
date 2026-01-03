using System.Threading.Channels;
using TraDe.Core;

namespace TraDe.Server;

// Decouples the Matching Engine from the SignalR Broadcaster
public class TradeNotificationChannel
{
    // Safety cap at 5,000 trades. If SignalR is slow, it drops frames (DropOldest) 
    private readonly Channel<Trade> _channel = Channel.CreateBounded<Trade>(new BoundedChannelOptions(5000)
    {
        FullMode = BoundedChannelFullMode.DropOldest, 
        SingleReader = true,
        SingleWriter = true
    });

    public ChannelReader<Trade> Reader => _channel.Reader;
    public ValueTask AddTradeAsync(Trade trade) => _channel.Writer.WriteAsync(trade);
}