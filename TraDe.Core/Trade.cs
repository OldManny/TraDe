namespace TraDe.Core;

// Represents an immutable execution between a buyer and a seller.
public record Trade(
    Guid Id,
    Guid BuyOrderId,
    Guid SellOrderId,
    decimal ExecutionPrice,
    decimal ExecutionQuantity,
    DateTime ExecutionTime
);