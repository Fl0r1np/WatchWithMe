using System.ComponentModel.DataAnnotations;

namespace WatchWithMeAPI.DTO;

public class UserNameUpdateRequestDTO
{
    [Required]
    public string UserName { get; set; }
}