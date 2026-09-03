using Auction.Domain.Bids;
using Auction.Domain.Items;
using Microsoft.EntityFrameworkCore;

namespace Auction.Infrastructure.Persistence;

public class AuctionDbContext(DbContextOptions<AuctionDbContext> options) : DbContext(options)
{
    public DbSet<Item> Items => Set<Item>();
    public DbSet<Bid> Bids => Set<Bid>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Item>(builder =>
        {
            builder.HasKey(item => item.Id);
            builder.Property(item => item.Title)
                .IsRequired()
                .HasMaxLength(200);
            builder.Property(item => item.StartingPrice).HasColumnType("decimal(18,2)");
            builder.Property(item => item.CurrentPrice).HasColumnType("decimal(18,2)");
            builder.Property(item => item.Status)
                .HasConversion<string>()
                .HasMaxLength(20);
            builder.Property(item => item.BlobUrl)
                .HasMaxLength(1000);
            builder.Property(item => item.RowVersion)
                .IsRowVersion();
        });

        modelBuilder.Entity<Bid>(builder =>
        {
            builder.HasKey(bid => bid.Id);
            builder.Property(bid => bid.Amount).HasColumnType("decimal(18,2)");
            builder.Property(bid => bid.BidderName)
                .IsRequired()
                .HasMaxLength(100);
            builder.HasOne<Item>()
                .WithMany()
                .HasForeignKey(bid => bid.ItemId);
        });
    }
}
