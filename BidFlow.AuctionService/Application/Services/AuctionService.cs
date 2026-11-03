using BidFlow.AuctionService.Application.Dtos;
using BidFlow.AuctionService.Domain;
using BidFlow.AuctionService.Domain.Entities;
using BidFlow.AuctionService.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BidFlow.AuctionService.Application.Services
{
    public class AuctionService : IAuctionService
    {
        private readonly AuctionDbContext _context;

        public AuctionService(AuctionDbContext context)
        {
            _context = context;
        }

        public async Task<Auction> CreateAuctionAsync(CreateAuctionDto dto, Guid SellerId)
        {
            if(string.IsNullOrWhiteSpace(dto.Title))
                throw new ArgumentException("Title is required.");
            if (string.IsNullOrWhiteSpace(dto.Description))
                throw new ArgumentException("Description is required.");
            if(dto.StartingPrice <= 0)
                throw new ArgumentException("Starting price must be greater than zero.");
            if(dto.StartTime < DateTime.UtcNow)
                throw new ArgumentException("Start time cannot be in the past.");
            if(dto.EndTime <= dto.StartTime)
                throw new ArgumentException("End time must be after start time.");
            var auction = new Auction
            {
                Title = dto.Title,
                Description = dto.Description,
                StartingPrice = dto.StartingPrice,
                CurrentPrice = dto.StartingPrice,
                StartingTime = dto.StartTime,
                EndTime = dto.EndTime,
                status = Domain.AuctionTypesEnums.Draft,
                SellerId = SellerId
            };
            await _context.Auctions.AddAsync(auction);
            await _context.SaveChangesAsync();
            return auction;
        }

        public async Task DeleteAuctionAsync(Guid auctionId, Guid sellerId)
        {
            var auction =await _context.Auctions.FirstOrDefaultAsync(a => a.AuctionId == auctionId && a.SellerId == sellerId);
            if (auction == null)
                throw new Exception("Cannot delete this auction");
            _context.Auctions.Remove(auction);
            await _context.SaveChangesAsync();
        }

        
        public async Task<List<Auction>> GetAllAuctionsAsync()
        {
            var Auctions = await _context.Auctions.AsNoTracking().Where(a=>
            a.status == AuctionTypesEnums.Open ||
            a.status == AuctionTypesEnums.Scheduled ||
            a.status == AuctionTypesEnums.Completed
            ).ToListAsync();
            if(Auctions == null)
                throw new Exception("No auctions found.");
            return Auctions;
        }
        
        public async Task<Auction> GetAuctionAsync(Guid Id,Guid? sellerId)
        {
            var auction =await _context.Auctions.FirstOrDefaultAsync(a => a.AuctionId == Id);
            if (auction == null)
                throw new Exception("Auction not found.");
            if(auction.status == AuctionTypesEnums.Draft || auction.status == AuctionTypesEnums.Cancelled)
            {
                if (auction.SellerId != sellerId)
                    return null;
            }
            return auction;
        }

        public async Task<Auction?> PublishAuctionAsync(Guid auctionId, Guid sellerId)
        {
            var auction =await _context.Auctions.FirstOrDefaultAsync(a => a.AuctionId == auctionId && a.SellerId == sellerId);
            if(auction == null)
                throw new Exception("Cannot publish this auction");
            if(auction.status != AuctionTypesEnums.Draft)
                throw new InvalidOperationException("Only draft auctions can be published.");
            var now = DateTime.UtcNow;
           
            if (auction.EndTime <= now)
                throw new InvalidOperationException(
                    "Auction has already expired.");
            auction.status = auction.StartingTime > now
                    ? AuctionTypesEnums.Scheduled
                    : AuctionTypesEnums.Open;
            await _context.SaveChangesAsync();
            return auction;
        }

        public async Task<Auction> UpdateAuctionAsync(Guid auctionId, UpdateAuctionDto dto, Guid sellerId)
        {
            var auction = await _context.Auctions.FirstOrDefaultAsync(a => a.AuctionId == auctionId && a.SellerId == sellerId);
            if(auction == null)
                throw new Exception("Cannot update this auction");
            if (auction.SellerId != sellerId)
                throw new UnauthorizedAccessException(
                    "You cannot update another user's auction.");

            if (auction.status != AuctionTypesEnums.Draft)
                throw new InvalidOperationException(
                    "Only draft auctions can be updated.");

            if (string.IsNullOrWhiteSpace(dto.Title))
                throw new ArgumentException("Title is required.");

            if (dto.StartingPrice <= 0)
                throw new ArgumentException("Starting price must be positive.");

            if (dto.StartTime <= DateTime.UtcNow)
                throw new ArgumentException("Start time must be in the future.");

            if (dto.EndTime <= dto.StartTime)
                throw new ArgumentException("End time must be after start time.");

            auction.Title = dto.Title;
            auction.Description = dto.Description;
            auction.StartingPrice = dto.StartingPrice;
            auction.CurrentPrice = dto.StartingPrice;
            auction.StartingTime = dto.StartTime;
            auction.EndTime = dto.EndTime;

            await _context.SaveChangesAsync();

            return auction;
        }

    }
}
