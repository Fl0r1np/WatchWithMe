namespace WatchWithMeAPI.Model;

public class CallParticipant
{
    public int Id { get; set; }
    
    // Foreign key to RoomParticipant(Id)
    public int RoomParticipantId { get; set; }

    public RoomParticipant RoomParticipant { get; set; } = null!;

    // Foreign key to CallSession(Id)
    public int CallSessionId  { get; set; }

    public CallSession CallSession { get; set; } = null!;

    public CallParticipantStatus Status { get; set; } = CallParticipantStatus.Ringing;

    public DateTime JoinedAt { get; set; }
    
    public DateTime LeftAt { get; set; }
    
}