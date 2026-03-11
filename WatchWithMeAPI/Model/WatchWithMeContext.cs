using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace WatchWithMeAPI.Model
{
    public class WatchWithMeContext(DbContextOptions<WatchWithMeContext> options)
        : IdentityDbContext<ApplicationUser>(options)
    {
        protected override void OnModelCreating(ModelBuilder builder)
        {
            // Necessary setup
            base.OnModelCreating(builder);
            builder.UseOpenIddict();
            
            
            
        }

    }
}
