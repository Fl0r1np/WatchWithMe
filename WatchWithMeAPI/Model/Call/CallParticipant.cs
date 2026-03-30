namespace WatchWithMeAPI.Model;

public class CallParticipant
{
    public int? Id { get; set; }
    
    public int? UserId { get; set; }
    
    public User? User { get; set; }

    public int? CallSessionId  { get; set; }

    public CallSession? CallSession { get; set; }

    public CallParticipantStatus? Status { get; set; }

    public DateTime? JoinedAt { get; set; }
    
    public DateTime? LeftAt { get; set; }
    
}