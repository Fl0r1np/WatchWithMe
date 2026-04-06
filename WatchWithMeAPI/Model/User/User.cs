using Microsoft.AspNetCore.Identity;

namespace WatchWithMeAPI.Model
{
    public class User : IdentityUser
    {
        
        public string ProfilePicture { get; set; } = "avatar-default.png";
        public UserStatus Status { get; set; } = UserStatus.Public;
        
        public UserStatus DisplayStatus { get; set; } = UserStatus.Online;

        public AuthMethod AuthenticationMethod { get; set; } = AuthMethod.Basic;

        // Collection Navigation Property
        // public ICollection<RoomParticipant> Participants { get; set; } = new HashSet<RoomParticipant>();


    }
}
