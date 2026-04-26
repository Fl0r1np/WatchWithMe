using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WatchWithMeAPI.Model;

public class RoomParticipant
{
    
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    
    // Foreign key to User(Id)
    public string UserId { get; set; } = null!;

    [ForeignKey(nameof(UserId))]
    public User User { get; set; } = null!;

    [Required]
    public string UserName { get; set; }

    // Foreign key to Room(Id)
    public int RoomId { get; set; }

    [ForeignKey(nameof(RoomId))]
    public Room Room { get; set; } = null!;

    public RoomParticipantRole Role { get; set; } = RoomParticipantRole.Viewer;

    public DateTime JoinedAt { get; set; } = DateTime.UtcNow;
    
    /*
    // Collection Navigation Property
    public ICollection<CallParticipant> CallParticipants { get; set; } = new HashSet<CallParticipant>();
    
    // Collection Navigation Property
    public ICollection<MessageReadState> MessageReadStates { get; set; } = new HashSet<MessageReadState>();
    */
}