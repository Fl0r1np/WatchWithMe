using Microsoft.AspNetCore.Identity;

namespace WatchWithMeAPI.Model
{
    public class User : IdentityUser
    {

        public string? DisplayName { get; set; }

        public string ProfilePicture { get; set; } = "avatar-default.png";

        public UserStatus? Status { get; set; }
        
    }
}
