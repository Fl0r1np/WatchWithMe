using Microsoft.EntityFrameworkCore;
using WatchWithMeAPI.DTOs.Room.Requests;
using WatchWithMeAPI.Model;
using WatchWithMeAPI.Services.NekoService;
using WatchWithMeAPI.Utils;

namespace WatchWithMeAPI.Services;

public class RoomService : IRoomService
{
    
    // Necessary services
    private readonly IConfiguration _configuration;
    private readonly WatchWithMeContext _context;
    private readonly ILogger<RoomService> _logger;
    private readonly INekoService _nekoService;

    public RoomService(
        IConfiguration configuration, 
        WatchWithMeContext context,
        ILogger<RoomService> logger,
        INekoService nekoService
        )
    {
        _configuration = configuration;
        _context = context;
        _logger = logger;
        _nekoService = nekoService;
    }

    /// <summary>
    /// Method to create a new room for the user with id equal to userId
    /// </summary>
    /// <param name="user">
    /// The user that creates the room
    /// </param>
    /// <param name="request">
    /// The DTO containing all room settings
    /// </param>
    /// <returns>
    /// Returns a string containing the Room ShareCode
    /// </returns>
    /// <exception cref="Exception">
    /// Throws an exception if there is a problem with the database or the creation of the room
    /// </exception>
    public async Task<Room> CreateRoomAsync(User user, CreateNewRoomRequestDTO request)
    {
        try
        {

            // User id
            var userId = user.Id;

            // Generate a unique ShareCode
            var shareCode = await GenerateUniqueShareCodeAsync();

            // Spin up the Docker container
            var nekoResult = await _nekoService.CreateAndStartNekoContainerAsync(shareCode);
            
            // Set the Docker data
            var containerId = nekoResult.ContainerId;
            var webRtcUrl = $"{_configuration["RoomData:WebRtcUrl"]}:{nekoResult.MappedPort}";


            // Create the Room object 
            var newRoom = new Room
            {
                ShareCode = shareCode,
                DisplayName = request.DisplayName,
                HostId = userId,
                NumberOfMaxParticipants = request.NumberOfMaxParticipants,
                DefaultParticipantRole = request.DefaultParticipantRole.ToEnum(RoomParticipantRole.Viewer),
                IsPrivate = request.IsPrivate,
                WebRtcUrl = webRtcUrl,
                ContainerId = containerId,
                ViewerPassword = nekoResult.ViewerPassword,
                HostPassword = nekoResult.HostPassword

            };

            // Create a RoomParticipant object for the ParticipantWithRoomControl
            var newRoomParticipant = new RoomParticipant
            {
                UserId = userId,
                UserName = user.UserName,
                Room = newRoom,
                Role = RoomParticipantRole.Host
            };

            // Add the newRoom to db
            _context.Rooms.Add(newRoom);

            // Save everything in one transaction
             var rowsAffected=  await _context.SaveChangesAsync();

             // Check if there was a problem 
             if (rowsAffected == 0)
             {
                 // The DB didn't crash, but it refused to save the data
                 _logger.LogWarning("Failed to create room for user {UserId}. Zero rows affected.", userId);
                 throw new Exception("The room could not be saved to the database.");
             }
             
             // Link the participant to Room's ParticipantWithRoomControl
             newRoom.ParticipantWithRoomControl = newRoomParticipant;

             // Save the changes
             await _context.SaveChangesAsync();
             
             _logger.LogInformation("Successfully created room {ShareCode} with Host {UserId}", shareCode, userId);
             return newRoom;
             
        }
        catch (DbUpdateException ex) // Catch specific EF Core errors
        {
            // This catches constraint violations, missing tables, etc.
            _logger.LogError(ex, "A database error occurred while creating a room for user {UserId}", user.Id);
            
            // Re-throw a cleaner exception that your Controller can handle
            throw new Exception("A database error occurred while creating the room. Please try again later.");
        }
        catch (Exception ex)
        {
            // This catches anything else (e.g., null reference exceptions)
            _logger.LogError(ex, "An unexpected error occurred in CreateRoomAsync");
            throw; 
        }
        

    }

    /// <summary>
    /// Method to join a room with a specific share code
    /// </summary>
    /// <param name="user">
    /// The user that wants to join the room
    /// </param>
    /// <param name="shareCode">
    /// The share code of the room
    /// </param>
    /// <returns>
    /// Returns the Room object
    /// </returns>
    /// <exception cref="Exception">
    /// Throws an exception if there is any problem with the database or the join of the room
    /// </exception>
    public async Task<Room> JoinRoomAsync(User user, string shareCode)
    {

        try
        {
            
            // Find the active room with the given share code
            var room = await _context.Rooms.Include(r => r.RoomParticipants).FirstOrDefaultAsync(r => r.ShareCode == shareCode && r.Status == RoomStatus.Active);
            
            // Check if the room exists
            if (room == null)
            {
                _logger.LogWarning("Failed to join room with shareCode = {ShareCode}. Room not found or is inactive.", shareCode);
                throw new Exception("The room with the given share code does not exist or is inactive.");
            }
            
            // Check if the user is already in the room
            if (room.RoomParticipants.Any(p => p.UserId == user.Id))
            {
                return room;
            }
            
            // Check if the room is full
            if (room.RoomParticipants.Count >= room.NumberOfMaxParticipants)
            {
                _logger.LogWarning("Failed to join room with shareCode = {ShareCode}. Room is full.", shareCode);
                throw new Exception("The room is full.");
            }
            
            // Create the RoomParticipant object
            var newRoomParticipant = new RoomParticipant
            {
                User = user,
                UserName = user.UserName ?? "Guest",
                Room = room,
                Role = room.DefaultParticipantRole
            };
            
            // Add the new participant to the room
            room.RoomParticipants.Add(newRoomParticipant);
            
            // Save the changes
            await _context.SaveChangesAsync();
            
            // Return the room
            _logger.LogInformation("The User with id = {UserId} has successfully joined room with shareCode = {ShareCode}", user.Id, shareCode);
            return room;

        }
        catch (DbUpdateException ex) // Catch specific EF Core errors
        {
            // This catches constraint violations, missing tables, etc.
            _logger.LogError(ex, "A database error occurred while joining the room with shareCode = {ShareCode}", shareCode);
            
            // Re-throw a cleaner exception that your Controller can handle
            throw new Exception("A database error occurred while joining the room. Please try again later.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unexpected error occurred in JoinRoomAsync");
            throw; 
        }
        
    }

    /// <summary>
    /// Method to find all participants in a room
    /// </summary>
    /// <param name="roomId">
    /// The id of the room
    /// </param>
    /// <returns>
    /// Returns a collection of RoomParticipant objects
    /// </returns>
    /// <exception cref="Exception">
    /// Throws an exception if there is a problem with the database
    /// </exception>
    public async Task<ICollection<RoomParticipant>> FindAllParticipantsByRoomIdAsync(int roomId)
    {

        try
        {
            
            // Find the room with the given id
            var room = await _context.Rooms.Include(r => r.RoomParticipants).FirstOrDefaultAsync(r => r.Id == roomId);

            // Check if the room exists
            if (room == null)
            {
                _logger.LogWarning("Failed to find all participants in room with id = {RoomId}. Room not found.", roomId);
                throw new Exception("The room with the given id does not exist.");
            }
            
            // Check if there is any participant in the room
            if (room.RoomParticipants.Count == 0)
            {
                _logger.LogWarning("Failed to find all participants in room with id = {RoomId}. Room is empty.", roomId);
                throw new Exception("The room is empty.");
            }
            
            // Return the participants
            _logger.LogInformation("Successfully found all participants in room with id = {RoomId}", roomId);
            return room.RoomParticipants;
            
        }
        catch (DbUpdateException ex)
        {
            _logger.LogError(ex, "A database error occurred while finding all participants in room with id = {RoomId}", roomId);
            throw;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "An unexpected error occurred in FindAllParticipantsByRoomIdAsync");
            throw;
        }
        
    }


    /// <summary>
    /// Method to generate a unique share code for a room
    /// </summary>
    /// <returns>
    /// A string containing a unique share code
    /// </returns>
    /// <exception cref="Exception">
    /// If the generator fails to generate a unique code
    /// </exception>
    private async Task<string> GenerateUniqueShareCodeAsync()
    {
        string shareCode;
        bool isUnique = false;
        int maxAttempts = 10; // Safety net to prevent infinite loops
        int attempts = 0;

        do
        {
            // Generate a random code
            shareCode = RoomServiceUtils.GenerateShareCode();

            // Check if ANY room in the database already has this code
            bool codeExists = await _context.Rooms.AnyAsync(r => r.ShareCode == shareCode);

            // If it doesn't exist, we found a unique one!
            if (!codeExists)
            {
                isUnique = true;
            }
        
            attempts++;

        } while (!isUnique && attempts < maxAttempts);

        if (!isUnique)
        {
            // If we tried 10 times and failed, something is deeply wrong with the generator
            _logger.LogError("Failed to generate a unique share code after {MaxAttempts} attempts.", maxAttempts);
            throw new Exception("Unable to generate a unique room code. Please try again.");
        }

        return shareCode;
    }
    
    
}