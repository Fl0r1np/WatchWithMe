namespace WatchWithMeAPI.DTO
{
    public class LoginResponseDTO
    {

        public string? Username { get; set; } 
        public string? Email { get; set; } 
        public string? AccessToken { get; set; }
        public int ExpiresIn { get; set; }

    }
}
