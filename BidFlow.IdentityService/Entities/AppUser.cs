using Microsoft.AspNetCore.Identity;

namespace BidFlow.IdentityService.Entities
{
    public class AppUser : IdentityUser<Guid>
    {
        public DateTime CreatedTime { get; set; } = DateTime.UtcNow;
    }
}
