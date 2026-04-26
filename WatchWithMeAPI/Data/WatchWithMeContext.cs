using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace WatchWithMeAPI.Model
{
    public class WatchWithMeContext(DbContextOptions<WatchWithMeContext> options)
        : IdentityDbContext<User>(options)
    {
        
        // Create the DBSets
        public DbSet<Room> Rooms { get; set; }
        public DbSet<RoomParticipant> RoomParticipants { get; set; }
        
        protected override void OnModelCreating(ModelBuilder builder)
        {
            // Necessary setup
            base.OnModelCreating(builder);
            builder.UseOpenIddict();
            
            // Set a default value for the User ProfilePicture
            builder.Entity<User>()
                .Property(u => u.ProfilePicture)
                .HasDefaultValue("avatar-default.png");
            
            // Fixing Multiple Cascade Paths
            
            // Relationship: Room -> RoomParticipant
            // Rule: If a Room is deleted, automatically delete all its Participants. (Cascade)
            builder.Entity<RoomParticipant>()
                .HasOne(rp => rp.Room)
                .WithMany(r => r.RoomParticipants)
                .HasForeignKey(rp => rp.RoomId)
                .OnDelete(DeleteBehavior.Restrict);
            
            // Relationship: User -> RoomParticipant
            // Rule: If a User is deleted, DO NOT automatically delete their participant records 
            builder.Entity<RoomParticipant>()
                .HasOne(rp => rp.User)
                .WithMany(u => u.Participants)
                .HasForeignKey(rp => rp.UserId)
                .OnDelete(DeleteBehavior.Restrict);
            
            // Relationship C: Room -> Host (User)
            // Rule: You cannot delete a User if they are currently the Host of a Room.
            // You must delete or reassign the Room first.
            builder.Entity<Room>()
                .HasOne(r => r.Host)
                .WithMany() 
                .HasForeignKey(r => r.HostId)
                .OnDelete(DeleteBehavior.Restrict);
            
            // Relationship D: Room -> ParticipantWithRoomControl (The Remote Control)
            // Rule: If the participant holding the "remote control" leaves or gets deleted,
            // don't delete the Room! Just set the room's remote control holder to NULL.
            builder.Entity<Room>()
                .HasOne(r => r.ParticipantWithRoomControl)
                .WithMany()
                .HasForeignKey(r => r.ParticipantWithRoomControlId)
                .OnDelete(DeleteBehavior.SetNull);
            
        }

        protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
        {
            base.ConfigureConventions(configurationBuilder);
            
            // Save 'UserStatus' enum as a string
            configurationBuilder
                .Properties<UserStatus>()
                .HaveConversion<string>();
            
            // Save 'AuthMethod' enum as a string
            configurationBuilder
                .Properties<AuthMethod>()
                .HaveConversion<string>();
            
            // Save 'RoomParticipantRole' enum as a string
            configurationBuilder
                .Properties<RoomParticipantRole>()
                .HaveConversion<string>();
            
            // Save 'RoomStatus' enum as a string
            configurationBuilder
                .Properties<RoomStatus>()
                .HaveConversion<string>();
            
        }
    }
}
