namespace TraDe.Core;

// Defines the side of the market.
public enum OrderSide { Buy, Sell }


// Defines the lifecycle of an order
public enum OrderStatus { Pending, Accepted, PartiallyFilled, Filled, Cancelled, Rejected }

public class Order
{
    // ID and Metadata (Immutable)
    public Guid Id { get; init; } = Guid.NewGuid();
    public DateTime CreationTime { get; init; } = DateTime.UtcNow;

    // Financial Data (Immutable: These do not change after creation)
    public decimal Price { get; init; }
    public decimal InitialQuantity { get; init; }
    public OrderSide Side { get; init; }

    // State Data (Mutable: These change as the engine processes the order)
    public decimal RemainingQuantity { get; private set; }
    public OrderStatus Status { get; private set; }

    // Static constraints
    private const decimal TickSize = 0.01m;
    private const decimal MinLotSize = 1.0m;

    // EF Core requires a parameterless constructor
    private Order() { }

    public Order(decimal price, decimal initialQuantity, OrderSide side)
    {
        if (price <= 0) throw new ArgumentException("Price must be positive.");
        if (initialQuantity < MinLotSize) throw new ArgumentException($"Quantity must be at least {MinLotSize}.");
        if (price % TickSize != 0) throw new ArgumentException($"Price must be a multiple of {TickSize}.");
        if (initialQuantity % MinLotSize != 0) throw new ArgumentException($"Quantity must be a multiple of {MinLotSize}.");

        Price = price;
        InitialQuantity = initialQuantity;
        RemainingQuantity = initialQuantity;
        Side = side;
        Status = OrderStatus.Accepted;
    }

    // Domain Logic: How an order is "decremented" during a match
    public void Fill(decimal quantity)
    {
        if (quantity <= 0) return;
        if (quantity > RemainingQuantity) 
            throw new InvalidOperationException("Overfill attempted.");
        
        RemainingQuantity -= quantity;
        Status = RemainingQuantity == 0 ? OrderStatus.Filled : OrderStatus.PartiallyFilled;
    }
}