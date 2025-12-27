using Microsoft.EntityFrameworkCore;
using TraDe.Core;

namespace TraDe.Server.Data;

public class TradingDbContext : DbContext
{
    public TradingDbContext(DbContextOptions<TradingDbContext> options) : base(options) { }

    public DbSet<Order> Orders => Set<Order>();
    public DbSet<Trade> Trades => Set<Trade>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Optimization: Precision for decimals
        modelBuilder.Entity<Order>().Property(o => o.Price).HasPrecision(18, 4);
        modelBuilder.Entity<Order>().Property(o => o.InitialQuantity).HasPrecision(18, 4);
        modelBuilder.Entity<Order>().Property(o => o.RemainingQuantity).HasPrecision(18, 4);
        
        modelBuilder.Entity<Trade>().Property(t => t.ExecutionPrice).HasPrecision(18, 4);
        modelBuilder.Entity<Trade>().Property(t => t.ExecutionQuantity).HasPrecision(18, 4);
        
        base.OnModelCreating(modelBuilder);
    }
}