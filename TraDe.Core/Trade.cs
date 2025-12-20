namespace TraDe.Core;

public class Trade
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid BuyOrderId { get; set; }
    public Guid SellOrderId { get; set; }
    public decimal Price { get; set; }
    public decimal Quantity { get; set; }
    public DateTime ExecutionTime { get; set; } = DateTime.UtcNow;
}