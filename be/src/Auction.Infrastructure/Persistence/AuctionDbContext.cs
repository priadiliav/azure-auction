using Auction.Domain.Bids;
using Auction.Domain.Interactions;
using Auction.Domain.Items;
using Auction.Domain.Users;
using Microsoft.EntityFrameworkCore;

namespace Auction.Infrastructure.Persistence;

public class AuctionDbContext(DbContextOptions<AuctionDbContext> options) : DbContext(options)
{
    public DbSet<Item> Items => Set<Item>();
    public DbSet<Bid> Bids => Set<Bid>();
    public DbSet<User> Users => Set<User>();
    public DbSet<Interaction> Interactions => Set<Interaction>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Item>(builder =>
        {
            builder.HasKey(item => item.Id);
            builder.Property(item => item.Title)
                .IsRequired()
                .HasMaxLength(200);
            builder.Property(item => item.Description)
                .IsRequired()
                .HasMaxLength(4000);
            builder.Property(item => item.StartingPrice).HasColumnType("decimal(18,2)");
            builder.Property(item => item.CurrentPrice).HasColumnType("decimal(18,2)");
            builder.Property(item => item.Status)
                .HasConversion<string>()
                .HasMaxLength(20);
            builder.Property(item => item.BlobUrl)
                .HasMaxLength(1000);
            builder.Property(item => item.SellerId)
                .IsRequired()
                .HasMaxLength(100);
            builder.Property(item => item.Embedding)
                .HasColumnType("nvarchar(max)");
            builder.Property(item => item.RowVersion)
                .IsRowVersion();
        });

        modelBuilder.Entity<Bid>(builder =>
        {
            builder.HasKey(bid => bid.Id);
            builder.Property(bid => bid.Amount).HasColumnType("decimal(18,2)");
            builder.Property(bid => bid.BidderId)
                .IsRequired()
                .HasMaxLength(100);
            builder.HasOne<Item>()
                .WithMany()
                .HasForeignKey(bid => bid.ItemId);
        });

        modelBuilder.Entity<User>(builder =>
        {
            builder.HasKey(user => user.Id);
            builder.Property(user => user.Id).HasMaxLength(100);
            builder.Property(user => user.Email).IsRequired().HasMaxLength(320);
            builder.Property(user => user.Name).IsRequired().HasMaxLength(200);
            builder.Property(user => user.AvatarUrl).HasMaxLength(1000);
        });

        modelBuilder.Entity<Interaction>(builder =>
        {
            builder.HasKey(interaction => interaction.Id);
            builder.Property(interaction => interaction.UserId)
                .IsRequired()
                .HasMaxLength(100);
            builder.Property(interaction => interaction.Type)
                .HasConversion<string>()
                .HasMaxLength(20);
            builder.HasIndex(interaction => interaction.UserId);
            builder.HasOne<Item>()
                .WithMany()
                .HasForeignKey(interaction => interaction.ItemId);
        });
    }
}
