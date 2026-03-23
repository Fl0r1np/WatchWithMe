using System.Security.Claims;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.JsonWebTokens;
using WatchWithMeAPI.DTO;
using WatchWithMeAPI.Model;
using WatchWithMeAPI.Services;

namespace WatchWithMeAPI.Controllers;

[Route("api/user")]
[ApiController]
public class UserController : ControllerBase
{
    
    // Necessary services
    private readonly SignInManager<User> _signInManager;
    private readonly UserManager<User> _userManager;
    private readonly JWTService _jwtService;
    
    // Validator
    private readonly IValidator<UpdateProfilePictureRequestDTO> _validatorProfilePictureRequest;
    
    /// <summary>
    /// Constructor of the class
    /// </summary>
    /// <param name="signInManager">
    /// API for user sing in 
    /// </param>
    /// <param name="userManager">
    /// Class containing all the logic for users repository management
    /// </param>
    /// <param name="jwtService">
    /// The service for JWT 
    /// </param>
    /// <param name="validatorProfilePictureRequest">
    /// Validator for the new Profile Picture
    /// </param>'
    public UserController(SignInManager<User> signInManager, UserManager<User> userManager, JWTService jwtService, IValidator<UpdateProfilePictureRequestDTO> validatorProfilePictureRequest)
    {
        _signInManager = signInManager;
        _userManager = userManager;
        _jwtService = jwtService;
        _validatorProfilePictureRequest = validatorProfilePictureRequest;
    }
    
    /// <summary>
    /// Updates the user's avatar'
    /// </summary>
    /// <param name="request">
    /// A DTO containing the new avatar's image name'
    /// </param>
    /// <returns>
    /// Returns a JSON data as a response
    /// </returns>
    [HttpPut("update-avatar")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async Task<IActionResult> UpdateAvatar([FromBody] UpdateProfilePictureRequestDTO request)
    {
        // Get the currently logged-in user's email from JWT
        var userEmail = User.FindFirstValue(JwtRegisteredClaimNames.Name);
        Console.WriteLine(userEmail);
        var user = await _userManager.FindByEmailAsync(userEmail);
        Console.WriteLine(user.DisplayName + " " + user.ProfilePicture);

        // Check if the user exists
        if (user == null)
        {
            return NotFound("User not found");
        }
        
        // Validates the new Profile Picture Filename
        var validationResult = await _validatorProfilePictureRequest.ValidateAsync(request);

        if (!validationResult.IsValid)
        {
            return BadRequest("Invalid Profile Picture!");
        }
        
        // Update the Profile Picture of the account
        user.ProfilePicture = request.AvatarFilename;
        await _userManager.UpdateAsync(user);
        
        return Ok( new { message = "Profile Picture updated successfully!" } );

    }

}