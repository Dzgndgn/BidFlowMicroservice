using BidFlow.AuctionService.Application.Dtos;
using BidFlow.AuctionService.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BidFlow.AuctionService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuctionsController : ControllerBase
    {
        private readonly IAuctionService _auctionService;

        public AuctionsController(IAuctionService auctionService)
        {
            _auctionService = auctionService;
        }
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> CreateAuction(CreateAuctionDto dto)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            if (!Guid.TryParse(userId, out var sellerId))
                return Unauthorized();
            try
            {
                var auction = await _auctionService.CreateAuctionAsync(dto, sellerId);
                return StatusCode(StatusCodes.Status201Created, auction);
            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [AllowAnonymous]
        [HttpGet("{Id}")]
        public async Task<IActionResult> GetAuctionById(Guid Id)
        {
            var auction = await _auctionService.GetAuctionAsync(Id);
            if (auction == null)
                return NotFound();
            return Ok(auction);
        }
        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> getAllAuction()
        {
            var auctions = await _auctionService.GetAllAuctionsAsync();
            if (auctions == null)
                return NotFound();
            return Ok(auctions);
        }
        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            if(!Guid.TryParse(userId, out var sellerId)){
                return Unauthorized();
            }
            await _auctionService.DeleteAuctionAsync(id, sellerId);
            return Ok("Auction deleted successfully.");
        }
        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAuctionAsync(Guid id,UpdateAuctionDto dto) {

            var userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            if (!Guid.TryParse(userId, out var sellerId))
                return Unauthorized();
            var updatedAuction = await _auctionService.UpdateAuctionAsync(id, dto, sellerId);
            if (updatedAuction == null)
                return NotFound();
            return Ok(updatedAuction);
        }
        [Authorize]
        [HttpPatch("{id}/publish")]
        public async Task<IActionResult> PublishAuction(Guid id)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            if (!Guid.TryParse(userId, out var sellerId))
                return Unauthorized();
            var publlishedAuction = await _auctionService.PublishAuctionAsync(id, sellerId);
            if (publlishedAuction == null)
                return BadRequest("Auction couldnt be published");
            return Ok(publlishedAuction);
        }

    }
}
