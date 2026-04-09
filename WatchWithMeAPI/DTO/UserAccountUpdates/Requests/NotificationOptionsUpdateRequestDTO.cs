using System.ComponentModel.DataAnnotations;

namespace WatchWithMeAPI.DTO;

public class NotificationOptionsUpdateRequestDTO
{

    [Required]
    public bool NotifyBasic { get; set; }

    [Required]
    public bool NotifyInvitations { get; set; }
    
}