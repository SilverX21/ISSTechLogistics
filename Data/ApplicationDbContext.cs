using ISSTechLogistics.Models.Orders;
using Microsoft.EntityFrameworkCore;

namespace ISSTechLogistics.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
    }

    public DbSet<Order> Orders { get; set; }

    public DbSet<OrderFile> OrderFiles { get; set; }

    public DbSet<OrdersDetailsStatistics> OrdersDetailsStatistics { get; set; }
}