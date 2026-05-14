using WatchWithMeAPI.Model;

namespace WatchWithMeAPI.DTOs;

public class RoomParticipantInfoDTO
{
    public int? Id { get; set; }

    public string? UserName { get; set; }

    public RoomParticipantRole? Role { get; set; }
}