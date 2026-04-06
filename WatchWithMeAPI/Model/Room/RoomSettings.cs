namespace WatchWithMeAPI.Model;

public class RoomSettings
{
    public int Id { get; set; }

    public int NumberOfMaxParticipants { get; set; }
    
    public bool IsPrivate { get; set; }

    public RoomParticipantRole DefaultParticipantRole { get; set; } = RoomParticipantRole.Viewer;
    
    // Foreign Key to Room(Id)
    public int RoomId { get; set; }

    public Room Room { get; set; } = null!;

}