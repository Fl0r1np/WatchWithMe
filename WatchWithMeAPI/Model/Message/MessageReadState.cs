namespace WatchWithMeAPI.Model;

public class MessageReadState
{
    public int Id { get; set; }

    // Foreign Key to Message(Id)
    public int MessageId { get; set; }

    public Message Message { get; set; } = null!;

    // Foreign Key to RoomParticipant(Id)
    public int RoomParticipantId { get; set; }

    public RoomParticipant RoomParticipant { get; set; } = null!;
    
    public DateTime ReadAt { get; set; }
}