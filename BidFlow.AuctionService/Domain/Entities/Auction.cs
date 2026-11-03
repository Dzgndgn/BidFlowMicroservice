namespace BidFlow.AuctionService.Domain.Entities
{
    public class Auction
    {
        public Guid AuctionId { get; set; } = Guid.NewGuid();
        public string Title { get; set; }
        public string Description { get; set; }
        public Guid SellerId { get; set; }
        public decimal StartingPrice { get; set; }
        public decimal CurrentPrice { get; set; }
        public DateTime StartingTime { get; set; }
        public DateTime EndTime { get; set; }
        public AuctionTypesEnums status { get; set; } = AuctionTypesEnums.Draft;
        public DateTime CreatedTime { get; set; } = DateTime.UtcNow;
    }
}