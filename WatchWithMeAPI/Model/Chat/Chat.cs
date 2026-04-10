namespace WatchWithMeAPI.Model;

public class Chat
{
    public int Id { get; set; }

    // Foreign Key to Room(Id)
    public int RoomId { get; set; }

    public Room Room { get; set; } = null!;

    // Collection Navigation Property
    public ICollection<Message> Messages { get; set; } = new HashSet<Message>();
    
    // Collection Navigation Property
    public ICollection<CallSession> CallSessions { get; set; } = new HashSet<CallSession>();
}