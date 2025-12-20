namespace TraDe.Core;

public enum OrderSide { Bid, Ask }
public enum OrderStatus { Created, Filled, Rejected }

public class Order
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public decimal Price { get; set; }
    public decimal InitialQuantity { get; set; }
    public decimal CurrentQuantity { get; set; }
    public OrderSide Side { get; set; }
    public OrderStatus Status { get; set; }
    public DateTime CreationTime { get; set; } = DateTime.UtcNow;

    // Helper property to check if order is fully filled
    public bool IsFilled => CurrentQuantity == 0;
}