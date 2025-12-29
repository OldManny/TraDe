using Microsoft.AspNetCore.Mvc;
using TraDe.Core;
using TraDe.Server.Models;

namespace TraDe.Server.Controllers;

[ApiController]
// API Versioning
[Route("api/v1/[controller]")] 
public class OrdersController : ControllerBase
{
    private readonly OrderProcessingChannel _channel;
    private readonly ILogger<OrdersController> _logger;

    public OrdersController(OrderProcessingChannel channel, ILogger<OrdersController> logger)
    {
        _channel = channel;
        _logger = logger;
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status202Accepted)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> PostOrder([FromBody] OrderRequest request)
    {
        // The Domain logic (Price/Qty validation) will throw an ArgumentException here
        var order = new Order(request.Price, request.Quantity, request.Side);

        var accepted = await _channel.TryAddOrderAsync(order);

        if (!accepted)
            return StatusCode(StatusCodes.Status503ServiceUnavailable, "Engine queue full.");

        // Returns 202 to signal the async nature of the engine
        return StatusCode(StatusCodes.Status202Accepted, new { 
            OrderId = order.Id, 
            ReceivedAt = order.CreationTime 
        });
    }
}