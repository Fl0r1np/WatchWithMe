using Microsoft.AspNetCore.Identity;

namespace WatchWithMeAPI.Model
{
    public class ApplicationUser : IdentityUser
    {

        public string? DisplayName { get; set; }

    }
}
