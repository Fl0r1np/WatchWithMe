namespace WatchWithMeAPI.Model;

public class RoomParticipant
{
    public int Id { get; set; }
    
    // Foreign key to User(Id)
    public int UserId { get; set; }

    public User User { get; set; } = null!;

    // Foreign key to Room(Id)
    public int RoomId { get; set; }

    public Room Room { get; set; } = null!;

    public RoomParticipantRole Role { get; set; } = RoomParticipantRole.Viewer;

    public DateTime JoinedAt { get; set; }
    
    // Collection Navigation Property
    public ICollection<CallParticipant> CallParticipants { get; set; } = new HashSet<CallParticipant>();
    
    // Collection Navigation Property
    public ICollection<MessageReadState> MessageReadStates { get; set; } = new HashSet<MessageReadState>();
    
}