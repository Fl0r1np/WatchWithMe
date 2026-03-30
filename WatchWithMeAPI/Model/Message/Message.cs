namespace WatchWithMeAPI.Model;

public abstract class Message
{

    public int? Id { get; set; }

    public int? ChatId { get; set; }

    public Chat? Chat { get; set; }

    public int? SenderId { get; set; }
    
    public User? Sender { get; set; }

    public DateTime? Timestamp { get; set; }

    public MessageStatus? Status { get; set; }

    public int? ReplyToMessageId { get; set; }
    
    public Message? ReplyToMessage { get; set; }

    public abstract void markAsRead();

}