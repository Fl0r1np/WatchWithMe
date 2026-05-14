using System.ComponentModel.DataAnnotations;

namespace WatchWithMeAPI.DTO
{
    public class RegisterRequestDTO
    {
        [Required]
        public string Username { get; set; } 

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [MinLength(8, ErrorMessage = "Password must be at least 8 characters long.")]
        public string Password { get; set; }

        [Required]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

    }
}

