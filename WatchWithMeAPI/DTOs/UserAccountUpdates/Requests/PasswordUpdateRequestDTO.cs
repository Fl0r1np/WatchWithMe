using System.ComponentModel.DataAnnotations;

namespace WatchWithMeAPI.DTO;

public class PasswordUpdateRequestDTO
{

    [Required]
    public string CurrentPassword { get; set; }
    
    [Required]
    public string NewPassword { get; set; }
    
}