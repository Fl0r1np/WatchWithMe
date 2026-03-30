using Microsoft.AspNetCore.Identity;

namespace WatchWithMeAPI.Model
{
    public class User : IdentityUser
    {

        public string ProfilePicture { get; set; } = "avatar-default.png";

        public UserStatus? Status { get; set; }
        
    }
}
