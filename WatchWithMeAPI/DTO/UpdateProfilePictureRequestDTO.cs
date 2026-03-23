using System.ComponentModel.DataAnnotations;

namespace WatchWithMeAPI.DTO;

public class UpdateProfilePictureRequestDTO
{
    [Required]
    public string AvatarFilename { get; set; }
}