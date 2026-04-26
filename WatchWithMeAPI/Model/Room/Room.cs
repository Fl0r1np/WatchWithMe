using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.CompilerServices;
using Microsoft.EntityFrameworkCore;

namespace WatchWithMeAPI.Model;

[Index(nameof(ShareCode), IsUnique = true)]
public class Room
{
    
    // Primary Key
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required]
    [StringLength(10)]
    public string ShareCode { get; set; } = null!;

    [Required]
    [StringLength(30, MinimumLength = 6)]
    public string DisplayName { get; set; } = null!;

    // Foreign Key to User(Id)
    public string HostId { get; set; } = null!;

    [ForeignKey(nameof(HostId))]
    public User Host { get; set; } = null!;

    // Collection Navigation Property
    public ICollection<RoomParticipant> RoomParticipants { get; set; } = new HashSet<RoomParticipant>();
    
    // Foreign Key to RoomParticipant(Id)
    public int? ParticipantWithRoomControlId { get; set; } = -1;
    
    [ForeignKey(nameof(ParticipantWithRoomControlId))]
    public RoomParticipant? ParticipantWithRoomControl { get; set; }

    public RoomStatus Status { get; set; } = RoomStatus.Active;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    [Range(2, 20)]
    public int NumberOfMaxParticipants { get; set; }
    
    public bool IsPrivate { get; set; }

    public RoomParticipantRole DefaultParticipantRole { get; set; } = RoomParticipantRole.Viewer;
    
    [Required]
    [MaxLength(2048)]
    [Url]
    public string WebRtcUrl { get; set; } = null!;

    [Required]
    [MaxLength(100)]
    public string ContainerId { get; set; } = null!;
    
}