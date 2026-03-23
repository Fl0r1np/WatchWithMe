namespace WatchWithMeAPI.Model;

public class Room
{

    public int? Id { get; set; }

    public string? ShareCode { get; set; }

    public string? DisplayName { get; set; }

    public int? RoomSettingsId { get; set; }
    
    public RoomSettings? RoomSettings { get; set; }

    public int? HostId { get; set; }
    
    public User? Host { get; set; }

    public List<RoomParticipant>? ListOfCurrentParticipants { get; set; }

    public RoomStatus? Status { get; set; }
    
    public DateTime? CreatedAt { get; set; }
    
}