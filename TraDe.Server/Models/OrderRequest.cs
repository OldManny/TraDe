using System.ComponentModel.DataAnnotations;
using TraDe.Core;

namespace TraDe.Server.Models;

public record OrderRequest(
    [Range(0.01, double.MaxValue)] decimal Price,
    [Range(1, double.MaxValue)] decimal Quantity,
    [Required] OrderSide Side
);