using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WatchWithMeAPI.Model;

public class Room
{

    public int Id { get; set; }

    public string ShareCode { get; set; } = null!;

    [Length(6, 30)]
    public string DisplayName { get; set; } = null!;

    // Navigation Property (The "One")
    public RoomSettings RoomSettings { get; set; } = null!;
    
    // Navigation Property (The "One")
    public RoomInstance RoomInstance { get; set; } = null!;

    // Foreign Key to User(Id)
    public int HostId { get; set; }

    public User Host { get; set; } = null!;

    // Collection Navigation Property
    public ICollection<RoomParticipant> RoomParticipants { get; set; } = new HashSet<RoomParticipant>();
    
    // Foreign Key to RoomParticipant(Id)
    public int CurrentControllerId { get; set; }
    
    public RoomParticipant CurrentController { get; set; } = null!;

    public RoomStatus Status { get; set; } = RoomStatus.Active;
    
    public DateTime CreatedAt { get; set; }
    
}