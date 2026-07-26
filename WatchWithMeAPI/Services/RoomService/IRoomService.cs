using WatchWithMeAPI.DTOs.Room.Requests;
using WatchWithMeAPI.Model;

namespace WatchWithMeAPI.Services;

public interface IRoomService
{
    
    Task<Room> CreateRoomAsync(User user, CreateNewRoomRequestDTO request);
    
    Task TerminateRoomAsync(int roomId);
    
    Task<Room> JoinRoomAsync(User user, string shareCode);
    
    Task<ICollection<RoomParticipant>> FindAllParticipantsByRoomIdAsync(int roomId);

}