using System.ComponentModel.DataAnnotations;

namespace WatchWithMeAPI.DTOs.Room.Requests;

public class CreateNewRoomRequestDTO
{

    [Required]
    [StringLength(30, MinimumLength = 6)]
    public string DisplayName { get; set; } = null!;
    
    [Range(2, 20)]
    public int NumberOfMaxParticipants { get; set; }
    
    [Required]
    public bool IsPrivate { get; set; }

    [Required]
    public string DefaultParticipantRole { get; set; }

}