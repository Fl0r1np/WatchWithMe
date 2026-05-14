using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.JsonWebTokens;
using WatchWithMeAPI.Model;
using WatchWithMeAPI.Model.VideoStream;

namespace WatchWithMeAPI.Hubs;

[Authorize]
public class RoomHub : Hub
{

    private readonly WatchWithMeContext _context;
    
    public RoomHub(WatchWithMeContext context)
    {
        _context = context;
    }

    // Front-End will call this after a user joins a room 
    /// <summary>
    /// Method to notify others about a new user joining a specific room 
    /// </summary>
    /// <param name="shareCode">
    /// A string containing the room's share code
    /// </param>
    public async Task JoinRoomGroup(string shareCode)
    {
        
        // Get the user's username from their JWT
        var username = Context.User?.FindFirstValue(JwtRegisteredClaimNames.Name) ?? "Guest";
        
        // Add their live WebSocket connection to the group named after share code
        await Groups.AddToGroupAsync(Context.ConnectionId, shareCode);
        
        // Instantly notify everybody else in the room
        await Clients.OthersInGroup(shareCode).SendAsync("UserJoined", username, "has joined the room.");

    }
    
    // Front-End will call this when a user leaves a room
    /// <summary>
    /// Method to notify others when a user leaves a room
    /// </summary>
    /// <param name="shareCode">
    /// A string containing the room's share code
    /// </param>
    public async Task LeaveRoomGroup(string shareCode)
    {
        
        // Get the user's username from their JWT
        var username = Context.User?.FindFirstValue(JwtRegisteredClaimNames.Name) ?? "Guest";
        
        // Remove their live WebSocket connection from the group
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, shareCode);
        
        // Notify others that the specific user left
        await Clients.OthersInGroup(shareCode).SendAsync("UserLeft", username, "has left the room.");

    }

    // Front-End will call this when a user sends a message to the chat 
    /// <summary>
    /// Method to notify others about a new message
    /// </summary>
    /// <param name="shareCode">
    /// A string containing the room's share code
    /// </param>
    /// <param name="message">
    /// A string containing the new message
    /// </param>
    public async Task SendChatMessage(string shareCode, string message)
    {
        
        // Get the user's username from their JWT
        var username = Context.User?.FindFirstValue(JwtRegisteredClaimNames.Name) ?? "Guest";
        
        // Brodcast the message to everyone in the room (including the sender)
        await Clients.Group(shareCode).SendAsync("ReceiveMessage", username, message);

    }
    
    // Front-End will call this when someone requests control of the room
    /// <summary>
    /// Method to notify others when someone requests the control for the room 
    /// </summary>
    /// <param name="shareCode">
    /// A string containig the room's share code
    /// </param>
    public async Task RequestControl(string shareCode)
    {
        // Get the user's username and their ID from their JWT
        var requesterUsername = Context.User?.FindFirstValue(JwtRegisteredClaimNames.Name) ?? "Guest";
        var requesterUserId = Context.User?.FindFirstValue(JwtRegisteredClaimNames.NameId) ?? "N/A";
        
        // Find the room and determine who holds the control
        var room = await _context.Rooms
            .Include(r => r.ParticipantWithRoomControl)
            .FirstOrDefaultAsync(r => r.ShareCode == shareCode);

        // Room doesn't exist
        if (room == null) return;
        
        // The admin requested the control back
        if (room.HostId == requesterUserId)
        {
            
            // Get the host
            var hostParticipant = room.RoomParticipants.FirstOrDefault(p => p.UserId == requesterUserId);

            if (hostParticipant != null)
            {
                // Update the user in control
                room.ParticipantWithRoomControlId = hostParticipant.Id;
                await _context.SaveChangesAsync();

                // Notify the other participants
                await Clients.Group(shareCode).SendAsync("ControlChanged", hostParticipant.UserName);
                return;

            }

        }
        
        // Nobody has the control now
        if (room.ParticipantWithRoomControlId == null)
        {
            // Get the participant and modify the room to have him in control
            var requesterParticipant = room.RoomParticipants.FirstOrDefault(p => p.UserId == requesterUserId);

            if (requesterParticipant != null)
            {

                room.ParticipantWithRoomControlId = requesterParticipant.Id;
                await _context.SaveChangesAsync();  
                
                // Notify the participants
                await Clients.Group(shareCode).SendAsync("ControlChanged", requesterUsername);
                return;

            }
        }   
        
        // Normal flow 
        var currentControllerUserId = room.ParticipantWithRoomControl.UserId;
        
        // Ask the current user in control
        await Clients.User(currentControllerUserId).SendAsync("ControlRequested", requesterUsername, requesterUserId);

    }

    // Front-End will call this when the current user who holds the control approves the request 
    /// <summary>
    /// Method to notify others when the current user who holds the control approves the request
    /// </summary>
    /// <param name="shareCode">
    /// A string containing the room's share code
    /// </param>
    /// <param name="approveUserId">
    /// A string containing the id of the user who approves the request
    /// </param>
    public async Task ApproveControlRequest(string shareCode, string approveUserId)
    {
        
        // Find the room and all it's participants
        var room = await _context.Rooms
            .Include(r => r.RoomParticipants)
            .FirstOrDefaultAsync(r => r.ShareCode == shareCode);
        
        // Check if the room exists
        if (room != null)
        {
            
            // Find the record of the participant that was just approved
            var participant = room.RoomParticipants
                .FirstOrDefault(r => r.UserId == approveUserId);
            
            // Check if the participant exists
            if (participant != null)
            {
                
                // Change the user in control and save in the database
                room.ParticipantWithRoomControl = participant;
                await _context.SaveChangesAsync();
                
                // Notify the room participants that the user in control has changed
                await Clients.Group(shareCode).SendAsync("ControlChanged", participant.UserName);

            }

        }

    }
    
    // Front-End will call this when someone changes the state of the stream
    /// <summary>
    /// Method to notify others when someone changes the state of the stream
    /// </summary>
    /// <param name="shareCode">
    /// A string containing the room's share code
    /// </param>
    /// <param name="state">
    /// The new state of the stream
    /// </param>
    /// <param name="currentTime">
    /// The time when the stream was changed
    /// </param>
    public async Task SyncVideoState(string shareCode, VideoStreamState state, double currentTime)
    {
        
        // Get the user's username from their JWT
        var username = Context.User?.FindFirstValue(JwtRegisteredClaimNames.Name) ?? "Guest";
        
        // Notify others about the request of control
        await Clients.OthersInGroup(shareCode).SendAsync("SyncVideoState", username, state, currentTime);
        
    }
    
    
    
    
    
}