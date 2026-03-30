using System.ComponentModel.DataAnnotations;
using WatchWithMeAPI.Model;

namespace WatchWithMeAPI.DTO;

public class StatusUpdateRequestDTO
{
    [Required]
    public string Status { get; set; }
}