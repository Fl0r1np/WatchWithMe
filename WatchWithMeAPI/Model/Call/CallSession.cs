namespace WatchWithMeAPI.Model;

public class CallSession
{
    public int? Id { get; set; }

    public int? ChatId { get; set; }

    public Chat? Chat { get; set; }

    public int? InitiatorId { get; set; }
    
    public User? Initiator { get; set; }

    public List<User> ListOfReceivers { get; set; }

    public CallStatus? Status { get; set; }

    public DateTime? StartedAt { get; set; }

    public DateTime? EndedAt { get; set; }
    
    
}