namespace WatchWithMeAPI.Model;

public class MessageReadState
{
    public int? Id { get; set; }

    public int? MessageId { get; set; }
    
    public Message? Message { get; set; }

    public int? UserId { get; set; }
    
    public User? User { get; set; }
    
    public DateTime? ReadAt { get; set; }
}