using BidFlow.AuctionService.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace BidFlow.AuctionService.Infrastructure.Persistence
{
    public class AuctionDbContext : DbContext
    {
        public AuctionDbContext(DbContextOptions<AuctionDbContext> options ) : base(options)
        {
        }
        public DbSet<Auction> Auctions { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Auction>(entitty =>
            {
                entitty.HasKey(a => a.AuctionId);
                entitty.Property(a => a.Title).IsRequired().HasMaxLength(200);
                entitty.Property(a => a.Description).IsRequired().HasMaxLength(2000);
                entitty.Property(a => a.StartingPrice).IsRequired().HasPrecision(18, 2);
                entitty.Property(a => a.CurrentPrice).IsRequired().HasPrecision(18, 2);
                entitty.Property(a => a.status).HasConversion<string>();
            });
        }
    }
    
}
