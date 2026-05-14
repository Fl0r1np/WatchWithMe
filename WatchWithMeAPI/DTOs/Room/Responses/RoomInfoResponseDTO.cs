namespace WatchWithMeAPI.DTOs.Room.Responses;

public class RoomInfoResponseDTO
{

    public int? Id { get; set; }

    public string? ShareCode { get; set; }

    public string? DisplayName { get; set; }

    public int? ParticipantWithRoomControlId { get; set; }

    public int? NumberOfMaxParticipants { get; set; }

    public string? WebRtcUrl { get; set; }
    
    public string? ContainerId { get; set; }

    public string? ViewerPassword { get; set; }

    public string? HostPassword { get; set; }

}