using TraDe.Core;

namespace TraDe.Tests;

public class OrderBookTests
{
    [Fact]
    public void OrderBook_ShouldMatch_FIFO_Priority()
    {
        var book = new OrderBook();
        var sell1 = new Order(100m, 10m, OrderSide.Sell);
        var sell2 = new Order(100m, 10m, OrderSide.Sell);
        var buy = new Order(100m, 15m, OrderSide.Buy);

        book.AddOrder(sell1);
        book.AddOrder(sell2);
        var trades = book.AddOrder(buy);

        Assert.Equal(2, trades.Count);
        Assert.Equal(10m, trades[0].ExecutionQuantity);
        Assert.Equal(sell1.Id, trades[0].SellOrderId); // Seller A matched first
        Assert.Equal(5m, trades[1].ExecutionQuantity);
        Assert.Equal(sell2.Id, trades[1].SellOrderId); // Seller B matched second
    }

    [Fact]
    public void OrderBook_CancelOrder_ShouldRemoveFromBook()
    {
        var book = new OrderBook();
        var buy = new Order(100m, 10m, OrderSide.Buy);
        book.AddOrder(buy);

        var cancelled = book.CancelOrder(buy.Id);
        var sell = new Order(100m, 10m, OrderSide.Sell);
        var trades = book.AddOrder(sell);

        Assert.True(cancelled);
        Assert.Empty(trades);
    }
}