using System.ComponentModel.DataAnnotations;

namespace WatchWithMeAPI.DTO;

public class DisplayStatusUpdateRequestDTO
{
    [Required]
    public string DisplayStatus { get; set; }
}