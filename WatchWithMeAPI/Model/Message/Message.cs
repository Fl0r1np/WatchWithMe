namespace WatchWithMeAPI.Model;

public abstract class Message
{

    public int Id { get; set; }

    // Foreign Key to Chat(Id)
    public int ChatId { get; set; }

    public Chat Chat { get; set; } = null!;

    // Foreign Key to RoomParticipant(Id)
    public int SenderId { get; set; }

    public RoomParticipant Sender { get; set; } = null!;

    public DateTime Timestamp { get; set; }

    public MessageStatus Status { get; set; } = MessageStatus.Sent;

    // Foreign Key to Message(Id)
    public int? ReplyToMessageId { get; set; }
    
    public Message? ReplyToMessage { get; set; }

    // Collection Navigation Property
    public ICollection<MessageReadState> MessageReadStates { get; set; } = new HashSet<MessageReadState>();
    public abstract void markAsRead();

}