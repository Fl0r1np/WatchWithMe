namespace WatchWithMeAPI.Model;

public class CallSession
{
    public int Id { get; set; }

    // Foreign Key to Chat(Id)
    public int ChatId { get; set; }

    public Chat Chat { get; set; } = null!;

    // Foreign Key to RoomParticipant(Id)
    public int InitiatorId { get; set; }

    public RoomParticipant Initiator { get; set; } = null!;

    // Collection Navigation Property
    public ICollection<CallParticipant> CallParticipants { get; set; } = new HashSet<CallParticipant>();

    public CallStatus Status { get; set; } = CallStatus.Connecting;

    public DateTime StartedAt { get; set; }

    public DateTime EndedAt { get; set; }
    
    
}