using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace WatchWithMeAPI.Model
{
    public class WatchWithMeContext : IdentityDbContext<ApplicationUser>
    {

        public WatchWithMeContext(DbContextOptions<WatchWithMeContext> options) : base(options)
        {
        
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.UseOpenIddict();
        }

        }
}
