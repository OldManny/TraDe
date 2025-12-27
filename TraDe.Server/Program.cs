using Microsoft.EntityFrameworkCore;
using TraDe.Core;
using TraDe.Server;
using TraDe.Server.Data;

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

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


var app = builder.Build();

// --- Middleware Pipeline ---

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// app.UseHttpsRedirection();
app.MapControllers();

app.Run();