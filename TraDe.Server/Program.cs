using TraDe.Core;
using TraDe.Server;

var builder = WebApplication.CreateBuilder(args);

// 1. Register the OrderBook as a Singleton (Only one exists for the whole app)
builder.Services.AddSingleton<OrderBook>();

// 2. Register the Communication Channel
builder.Services.AddSingleton<OrderProcessingChannel>();

// 3. Register the Background Worker that runs the matching engine
builder.Services.AddHostedService<MatchingEngineWorker>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.MapControllers();

app.Run();