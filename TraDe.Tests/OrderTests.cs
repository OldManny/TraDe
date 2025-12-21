using TraDe.Core;

namespace TraDe.Tests;

public class OrderTests
{
    [Fact]
    public void Order_InvalidTickSize_ShouldThrowException()
    {
        // Price of 100.005 is invalid if TickSize is 0.01
        Assert.Throws<ArgumentException>(() => new Order(100.005m, 10m, OrderSide.Buy));
    }

    [Fact]
    public void Order_ValidOrder_ShouldBeAccepted()
    {
        var order = new Order(100.01m, 10m, OrderSide.Buy);
        Assert.Equal(OrderStatus.Accepted, order.Status);
    }

    [Fact]
    public void Order_Fill_FullLogic_ShouldPass()
    {
        var order = new Order(100m, 10m, OrderSide.Buy);
        order.Fill(10m);
        Assert.Equal(0, order.RemainingQuantity);
        Assert.Equal(OrderStatus.Filled, order.Status);
    }
}