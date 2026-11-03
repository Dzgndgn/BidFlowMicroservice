namespace BidFlow.AuctionService.Application.Dtos
{
    public class UpdateAuctionDto
    {
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public decimal StartingPrice { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
    }
}
