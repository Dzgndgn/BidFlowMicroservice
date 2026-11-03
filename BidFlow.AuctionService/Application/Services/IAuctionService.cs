using BidFlow.AuctionService.Application.Dtos;
using BidFlow.AuctionService.Domain.Entities;

namespace BidFlow.AuctionService.Application.Services
{
    public interface IAuctionService
    {
        Task<Auction> CreateAuctionAsync(CreateAuctionDto dto, Guid SellerId);
        Task<Auction> GetAuctionAsync(Guid auctionId,Guid? sellerId);
        Task<List<Auction>> GetAllAuctionsAsync();
        Task<Auction> UpdateAuctionAsync(Guid auctionId, UpdateAuctionDto dto,Guid sellerId);
        Task DeleteAuctionAsync(Guid auctionId ,Guid sellerId);
        Task<Auction?> PublishAuctionAsync(Guid auctionId, Guid sellerId);
    }
}
