using Microsoft.EntityFrameworkCore;
using TraDe.Core;
using TraDe.Server;
using TraDe.Server.Data;
using TraDe.Server.Middleware;

var builder = WebApplication.CreateBuilder(args);

// --- Configuration & Services Registration ---

// Database Configuration (Uses Environment Variables if present)
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<TradingDbContext>(options =>
    options.UseNpgsql(connectionString, npgsqlOptions => 
    {
        // Handle transient failures (Network blips, DB restarts)
        npgsqlOptions.EnableRetryOnFailure(
            maxRetryCount: 5,
            maxRetryDelay: TimeSpan.FromSeconds(30),
            errorCodesToAdd: null);
    }));

// Core Domain Logic
builder.Services.AddSingleton<OrderBook>();

// High-Performance Channels (Decoupling)
builder.Services.AddSingleton<OrderProcessingChannel>();
builder.Services.AddSingleton<TradePersistenceChannel>();

// Background Workers (Actors)
builder.Services.AddHostedService<MatchingEngineWorker>();
builder.Services.AddHostedService<PersistenceWorker>();
builder.Services.AddHostedService<MarketDataReplayService>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


var app = builder.Build();

app.UseMiddleware<ExceptionMiddleware>();

// --- Middleware Pipeline ---

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();

// Auto-Migrate on Startup
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<TradingDbContext>();
        context.Database.Migrate();
        Console.WriteLine("Database migrations applied successfully.");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"An error occurred while migrating the database: {ex.Message}");
    }
}

app.Run();

// The entry point to the integration tests
public partial class Program { }