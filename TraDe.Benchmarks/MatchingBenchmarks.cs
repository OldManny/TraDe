using BenchmarkDotNet.Attributes;
using TraDe.Core;

namespace TraDe.Benchmarks;

[MemoryDiagnoser]
public class MatchingBenchmarks
{
    private List<Order> _orders = null!;

    [GlobalSetup]
    public void Setup()
    {
        _orders = new List<Order>();
        for (int i = 0; i < 100_000; i++)
        {
            // Using positional arguments for Order constructor
            _orders.Add(new Order(
                100m + (i % 10), 
                10m, 
                i % 2 == 0 ? OrderSide.Buy : OrderSide.Sell));
        }
    }

    [Benchmark]
    public void Match100kOrders()
    {
        var book = new OrderBook();
        
        foreach (var order in _orders)
        {
            book.AddOrder(order);
        }
    }
}