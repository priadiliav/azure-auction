using Auction.Domain.Items;
using Microsoft.EntityFrameworkCore;

namespace Auction.Infrastructure.Persistence;

public class AuctionDbContext(DbContextOptions<AuctionDbContext> options) : DbContext(options)
{
    public DbSet<Item> Items => Set<Item>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Item>(builder =>
        {
            builder.HasKey(item => item.Id);
            builder.Property(item => item.Title)
                .IsRequired()
                .HasMaxLength(200);
            builder.Property(item => item.StartingPrice).HasColumnType("decimal(18,2)");
            builder.Property(item => item.Status)
                .HasConversion<string>()
                .HasMaxLength(20);
        });
    }
}
