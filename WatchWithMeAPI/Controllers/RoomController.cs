using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WatchWithMeAPI.DTOs;
using WatchWithMeAPI.DTOs.Room.Requests;
using WatchWithMeAPI.DTOs.Room.Responses;
using WatchWithMeAPI.Model;
using WatchWithMeAPI.Services;

namespace WatchWithMeAPI.Controllers;

[ApiController]
[Route("api/room")]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class RoomController : ControllerBase
{
    
    // Necessary constants
    private const string USER_NOT_FOUND = "User not found!";
    
    // Necessary services
    private readonly IRoomService _roomService;
    private readonly UserManager<User> _userManager;
    
    // Validator
    private readonly IValidator<CreateNewRoomRequestDTO> _validatorCreateNewRoomRequest;
    private readonly IValidator<JoinRoomWithShareCodeRequestDTO> _validatorJoinRoomWithShareCodeRequest;
    
    public RoomController(
        IRoomService roomService, 
        UserManager<User> userManager,
        IValidator<CreateNewRoomRequestDTO> validatorCreateNewRoomRequest,
        IValidator<JoinRoomWithShareCodeRequestDTO> validatorJoinRoomWithShareCodeRequest
        )
    {
        _roomService = roomService;
        _userManager = userManager;
        _validatorCreateNewRoomRequest = validatorCreateNewRoomRequest;
        _validatorJoinRoomWithShareCodeRequest = validatorJoinRoomWithShareCodeRequest;
    }

    [HttpPost("create-new-room")]
    public async Task<ActionResult<RoomInfoResponseDTO>> CreateNewRoom([FromBody] CreateNewRoomRequestDTO request)
    {
        
        // Get the currently logged-in user's email from JWT
        var userEmail = User.FindFirstValue(JwtRegisteredClaimNames.Email);
        var user = await _userManager.FindByEmailAsync(userEmail);
        
        // Check if the user exists
        if (user == null)
        {
            return NotFound(USER_NOT_FOUND);
        }
        
        // Validates the request
        var validationResult = _validatorCreateNewRoomRequest.Validate(request);

        if (!validationResult.IsValid)
        {
            return ValidationProblem(new ValidationProblemDetails(validationResult.ToDictionary()));
        }
        
        // Try to create the new room
        try
        {
            
            // Create the new room
            var result = await _roomService.CreateRoomAsync(user, request);
            
            // Return result info
            return new RoomInfoResponseDTO
            {
                Id = result.Id,
                ShareCode = result.ShareCode,
                DisplayName = result.DisplayName,
                ParticipantWithRoomControlId = result.ParticipantWithRoomControlId,
                NumberOfMaxParticipants = result.NumberOfMaxParticipants,
                WebRtcUrl = result.WebRtcUrl,
                ContainerId = result.ContainerId,
                ViewerPassword = result.ViewerPassword,
                HostPassword = result.HostPassword
            };

        }
        catch (Exception e)
        {
            return Problem(e.Message);
        }

    }

    [HttpPost("join-room-with-share-code")]
    public async Task<ActionResult<RoomInfoResponseDTO>> JoinRoomWithShareCode([FromBody] JoinRoomWithShareCodeRequestDTO request)
    {
        
        // Get the currently logged-in user's email from JWT
        var userEmail = User.FindFirstValue(JwtRegisteredClaimNames.Email);
        var user = await _userManager.FindByEmailAsync(userEmail);
        
        // Check if the user exists
        if (user == null)
        {
            return NotFound(USER_NOT_FOUND);
        }
        
        // Validates the request
        var validationResult = _validatorJoinRoomWithShareCodeRequest.Validate(request);

        if (!validationResult.IsValid)
        {
            return ValidationProblem(new ValidationProblemDetails(validationResult.ToDictionary()));
        }

        try
        {
            
            // Try to join the room
            var room = await _roomService.JoinRoomAsync(user, request.ShareCode);
            
            // Return result info
            return new RoomInfoResponseDTO
            {
                Id = room.Id,
                ShareCode = room.ShareCode,
                DisplayName = room.DisplayName,
                ParticipantWithRoomControlId = room.ParticipantWithRoomControlId,
                NumberOfMaxParticipants = room.NumberOfMaxParticipants,
                WebRtcUrl = room.WebRtcUrl,
                ContainerId = room.ContainerId
            };

        }
        catch (Exception e)
        {
            return Problem(e.Message);
        }

    }
    
    [HttpGet("{roomId:int}/participants")]
    public async Task<ActionResult<AllRoomParticipantsResponseDTO>> GetAllRoomParticipants([FromRoute] int roomId)
    {
        
        try
        {

            // Get all participants
            var participants = await _roomService.FindAllParticipantsByRoomIdAsync(roomId);
            
            // Create the response
            var response = new AllRoomParticipantsResponseDTO
            {
                RoomParticipants = new List<RoomParticipantInfoDTO>()
            };
            
            // Check if there are any participants
            if (participants.Count == 0)
            {
                return Ok( new { 
                    message = "No participants found in this room."
                });
            }
            
            // Adding the participants to the response
            foreach (var participant in participants)
            {
                
                response.RoomParticipants.Add(
                        new RoomParticipantInfoDTO
                        {
                            Id = participant.Id,
                            UserName = participant.UserName,
                            Role = participant.Role
                        }
                    );
                
            }
            
            // Return the response
            return response;

        }
        catch (Exception e)
        {
            return Problem(e.Message);
        }
        
    }

}