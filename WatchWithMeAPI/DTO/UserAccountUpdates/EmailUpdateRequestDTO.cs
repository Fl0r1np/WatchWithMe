using System.ComponentModel.DataAnnotations;

namespace WatchWithMeAPI.DTO;

public class EmailUpdateRequestDTO
{
    [Required]
    [EmailAddress]
    public string Email { get; set; }
}