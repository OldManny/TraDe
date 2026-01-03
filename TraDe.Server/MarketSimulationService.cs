using TraDe.Core;

namespace TraDe.Server;

public class MarketSimulationService : BackgroundService
{
    private readonly OrderProcessingChannel _orderChannel;
    private readonly ILogger<MarketSimulationService> _logger;
    private readonly Random _random = new();

    // Configuration
    private decimal _currentPrice = 100.00m;
    private const decimal Volatility = 0.50m; // Max price jump
    
    public MarketSimulationService(
        OrderProcessingChannel orderChannel, 
        ILogger<MarketSimulationService> logger)
    {
        _orderChannel = orderChannel;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        // Wait for Engine to boot
        await Task.Delay(3000, stoppingToken);
        _logger.LogInformation("Starting Market Simulation (Market Maker)...");

        // Safety: Tick every 100ms (10 updates/sec). 
        using var timer = new PeriodicTimer(TimeSpan.FromMilliseconds(100));

        while (await timer.WaitForNextTickAsync(stoppingToken))
        {
            // 1. Random Walk (Brownian Motion)
            var change = (decimal)(_random.NextDouble() * 2 - 1) * Volatility;
            _currentPrice += change;
            if (_currentPrice < 10) _currentPrice = 10; // Floor

            // 2. Generate Orders (1 Buy, 1 Sell around the price)
            var buyPrice = Math.Round(_currentPrice - 0.05m, 2);
            var sellPrice = Math.Round(_currentPrice + 0.05m, 2);
            var qty = _random.Next(10, 100);

            var buyOrder = new Order(buyPrice, qty, OrderSide.Buy);
            var sellOrder = new Order(sellPrice, qty, OrderSide.Sell);

            // 3. Push to Engine
            // Note: TryAddOrderAsync will WAIT if the engine is full.
            await _orderChannel.TryAddOrderAsync(buyOrder, stoppingToken);
            await _orderChannel.TryAddOrderAsync(sellOrder, stoppingToken);

            // 4. Cross the spread to ensure Trades happen
            if (_random.NextDouble() > 0.7) 
            {
                var marketActionSide = _random.NextDouble() > 0.5 ? OrderSide.Buy : OrderSide.Sell;
                var aggressivePrice = marketActionSide == OrderSide.Buy ? sellPrice : buyPrice;
                var aggressiveOrder = new Order(aggressivePrice, qty, marketActionSide);
                await _orderChannel.TryAddOrderAsync(aggressiveOrder, stoppingToken);
            }
        }
    }
}