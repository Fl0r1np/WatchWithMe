using System.ComponentModel.DataAnnotations;

namespace WatchWithMeAPI.DTO;

public class ProfilePictureUpdateRequestDTO
{
    [Required]
    public string ProfilePictureFilename { get; set; }
}