using BidFlow.IdentityService.Application.Dtos;
using BidFlow.IdentityService.Application.Services;
using BidFlow.IdentityService.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BidFlow.IdentityService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly TokenService _tokenService;
        public AuthController(UserManager<AppUser> userManager, TokenService tokenService)
        {
            _userManager = userManager;
            _tokenService = tokenService;
        }
        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDto dto)
        {
            var user = new AppUser
            {
                UserName = dto.Username,
                Email = dto.Email
            };
            var resultUser = await _userManager.CreateAsync(user, dto.Password);
            if(!resultUser.Succeeded)
                return BadRequest(resultUser.Errors);
            return StatusCode(StatusCodes.Status201Created, new
            {
                Message = "User created successfully",
                UserId = user.Id
            });
        }
        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto dto)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);
            if(user == null)
                return Unauthorized("Invalid email or password");
            var isPasswordTrue = await _userManager.CheckPasswordAsync(user, dto.Password);
            if(!isPasswordTrue)
                return Unauthorized("Invalid email or password");
            var token = _tokenService.GenerateToken(user);
            return Ok(new
            {
                AccesToken = token,
                TokenType = "Bearer"
            });
        }

        [Authorize]
        [HttpGet("profile")]
        public async Task<IActionResult> GetProfile()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            var email = User.FindFirst(ClaimTypes.Email).Value;
            var username = User.FindFirst(ClaimTypes.Name).Value;
            return Ok(new
            {
                UserId = userId,
                Email = email,
                Username = username
            });
        }
    }
}
