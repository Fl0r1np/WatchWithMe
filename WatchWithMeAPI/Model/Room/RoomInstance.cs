namespace WatchWithMeAPI.Model;

public class RoomInstance
{
    public int Id { get; set; }

    // Foreign key to Room(Id)
    public int RoomId { get; set; }

    public Room Room { get; set; } = null!;

    public string WebRtcUrl { get; set; } = null!;

    public string ContainerId { get; set; } = null!;
}