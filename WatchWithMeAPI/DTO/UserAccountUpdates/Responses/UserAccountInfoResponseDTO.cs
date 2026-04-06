using WatchWithMeAPI.Model;

namespace WatchWithMeAPI.DTO;

public class UserAccountInfoResponseDTO
{
    public string? Username { get; set; }
    public string? Email { get; set; }
    public string? ProfilePicture { get; set; }
    public string? Status { get; set; }
    public string? AuthMethod { get; set; }
    
}