namespace WatchWithMeAPI.Model;

public class RoomParticipant
{
    public int? Id { get; set; }
    
    public int? UserId { get; set; }
    
    public User? User { get; set; }
    
    public int? RoomId { get; set; }

    public Room? Room { get; set; }

    public RoomParticipantRole? Role { get; set; }

    public DateTime? JoinedAt { get; set; }
}