namespace WatchWithMeAPI.Model;

public class RoomInstance
{
    public int? Id { get; set; }

    public int? RoomId { get; set; }
    
    public Room? Room { get; set; }
    
    public string? WebRtcUrl { get; set; }

    public string? ContainerId { get; set; }
}