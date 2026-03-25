using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace WatchWithMeAPI.Model
{
    public class WatchWithMeContext(DbContextOptions<WatchWithMeContext> options)
        : IdentityDbContext<User>(options)
    {
        protected override void OnModelCreating(ModelBuilder builder)
        {
            // Necessary setup
            base.OnModelCreating(builder);
            builder.UseOpenIddict();
            
            // Set a default value for the User ProfilePicture
            builder.Entity<User>()
                .Property(u => u.ProfilePicture)
                .HasDefaultValue("avatar-default.png");
            
        }

    }
}
