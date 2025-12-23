namespace TraDe.Core;

public class OrderBook
{
    private readonly SortedDictionary<decimal, LinkedList<Order>> _bids;
    private readonly SortedDictionary<decimal, LinkedList<Order>> _asks;
    
    // Fast lookup for Cancellations O(1)
    private readonly Dictionary<Guid, Order> _orders = new();

    public OrderBook()
    {
        _bids = new SortedDictionary<decimal, LinkedList<Order>>(new DescendingComparer());
        _asks = new SortedDictionary<decimal, LinkedList<Order>>();
    }

    public List<Trade> AddOrder(Order order)
    {
        var trades = new List<Trade>();
        _orders.Add(order.Id, order);

        if (order.Side == OrderSide.Buy)
        {
            MatchOrder(order, _asks, trades);
            if (order.RemainingQuantity > 0) PlaceRestingOrder(order, _bids);
        }
        else
        {
            MatchOrder(order, _bids, trades);
            if (order.RemainingQuantity > 0) PlaceRestingOrder(order, _asks);
        }

        // If order was fully filled, no need to track it for cancellation
        if (order.Status == OrderStatus.Filled) _orders.Remove(order.Id);

        return trades;
    }

    // Cancellation Logic
    public bool CancelOrder(Guid orderId)
    {
        if (!_orders.TryGetValue(orderId, out var order)) return false;

        var book = order.Side == OrderSide.Buy ? _bids : _asks;
        if (book.TryGetValue(order.Price, out var list))
        {
            list.Remove(order);
            if (list.Count == 0) book.Remove(order.Price);
        }

        _orders.Remove(orderId);
        return true;
    }

    private void MatchOrder(Order order, SortedDictionary<decimal, LinkedList<Order>> oppositeBook, List<Trade> trades)
    {
        // Iterate without .ToList() to save memory/latency
        while (oppositeBook.Count > 0 && order.RemainingQuantity > 0)
        {
            var bestPriceLevel = oppositeBook.First();
            decimal price = bestPriceLevel.Key;
            
            // Check if price crosses
            bool isMatch = order.Side == OrderSide.Buy ? order.Price >= price : order.Price <= price;
            if (!isMatch) break;

            var listAtPrice = bestPriceLevel.Value;
            while (listAtPrice.First is { Value: var restingOrder } && order.RemainingQuantity > 0)
            {
                var matchQty = Math.Min(order.RemainingQuantity, restingOrder.RemainingQuantity);

                trades.Add(new Trade(Guid.NewGuid(), 
                    order.Side == OrderSide.Buy ? order.Id : restingOrder.Id,
                    order.Side == OrderSide.Sell ? order.Id : restingOrder.Id,
                    price, matchQty, DateTime.UtcNow));

                order.Fill(matchQty);
                restingOrder.Fill(matchQty);

                if (restingOrder.Status == OrderStatus.Filled)
                {
                    listAtPrice.RemoveFirst();
                    _orders.Remove(restingOrder.Id); // Clean up lookup map
                }
            }

            if (listAtPrice.Count == 0) oppositeBook.Remove(price);
        }
    }

    private void PlaceRestingOrder(Order order, SortedDictionary<decimal, LinkedList<Order>> book)
    {
        if (!book.TryGetValue(order.Price, out var list))
        {
            list = new LinkedList<Order>();
            book.Add(order.Price, list);
        }
        list.AddLast(order);
    }

    private class DescendingComparer : IComparer<decimal>
    {
        public int Compare(decimal x, decimal y) => y.CompareTo(x);
    }
}