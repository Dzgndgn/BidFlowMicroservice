using BidFlow.IdentityService.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BidFlow.IdentityService.Context
{
    public class AppIdentityDbContext : IdentityDbContext<AppUser,IdentityRole<Guid>,Guid>
    {
        public AppIdentityDbContext(DbContextOptions<AppIdentityDbContext> options) : base(options) 
        {
        }
    }
}
