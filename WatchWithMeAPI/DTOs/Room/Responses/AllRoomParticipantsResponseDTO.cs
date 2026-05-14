namespace WatchWithMeAPI.DTOs.Room.Responses;

public class AllRoomParticipantsResponseDTO
{
    public ICollection<RoomParticipantInfoDTO>? RoomParticipants { get; set; }
}