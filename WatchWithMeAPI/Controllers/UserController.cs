using System.Security.Claims;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.JsonWebTokens;
using WatchWithMeAPI.DTO;
using WatchWithMeAPI.Model;
using WatchWithMeAPI.ResponseRecords;
using WatchWithMeAPI.Services;
using WatchWithMeAPI.Utils;
using WatchWithMeAPI.Validators;

namespace WatchWithMeAPI.Controllers;

[Route("api/user")]
[ApiController]
public class UserController : ControllerBase
{

    // Necessary services
    private readonly UserManager<User> _userManager;

    // Validator
    private readonly IValidator<ProfilePictureUpdateRequestDTO> _validatorProfilePictureRequest;
    private readonly IValidator<EmailUpdateRequestDTO> _validatorEmailUpdateRequest;
    private readonly IValidator<PasswordUpdateRequestDTO> _validatorPasswordUpdateRequest;
    private readonly IValidator<UserNameUpdateRequestDTO> _validatorUserNameUpdateRequest;
    private readonly IValidator<StatusUpdateRequestDTO> _validatorStatusUpdateRequest;
    private readonly IValidator<DisplayStatusUpdateRequestDTO> _validatorDisplayStatusUpdateRequest;
    private readonly IValidator<NotificationOptionsUpdateRequestDTO> _validatorNotificationOptionsUpdateRequest;

    // Necessary constants
    private const string USER_NOT_FOUND = "User not found!";

    /// <summary>
    /// Constructor of the class
    /// </summary>
    /// <param name="userManager">
    /// Class containing all the logic for users repository management
    /// </param>
    /// <param name="validatorProfilePictureRequest">
    /// Validator for the new Profile Picture
    /// </param>'
    /// <param name="validatorEmailUpdateRequest">
    /// Validator for the new Email
    /// </param>
    /// <param name="validatorPasswordUpdateRequest">
    /// Validator for the new Password
    /// </param>
    /// <param name="validatorUserNameUpdateRequest">
    /// Validator for the new UserName
    /// </param>
    /// <param name="validatorStatusUpdateRequest">
    /// Validator for the new Status
    /// </param>
    /// <param name="validatorDisplayStatusUpdateRequest">
    /// Validator for the new Display Status
    /// </param>
    /// <param name="validatorNotificationOptionsUpdateRequest">
    /// Validator for the new Notification Options
    /// </param>
    public UserController(
        UserManager<User> userManager,
        IValidator<ProfilePictureUpdateRequestDTO> validatorProfilePictureRequest,
        IValidator<EmailUpdateRequestDTO> validatorEmailUpdateRequest,
        IValidator<PasswordUpdateRequestDTO> validatorPasswordUpdateRequest,
        IValidator<UserNameUpdateRequestDTO> validatorUserNameUpdateRequest,
        IValidator<StatusUpdateRequestDTO> validatorStatusUpdateRequest,
        IValidator<DisplayStatusUpdateRequestDTO> validatorDisplayStatusUpdateRequest,
        IValidator<NotificationOptionsUpdateRequestDTO> validatorNotificationOptionsUpdateRequest)
    {

        // Asing the necessary services to the class
        _userManager = userManager;

        // Assigning the necessary validators to the class
        _validatorProfilePictureRequest = validatorProfilePictureRequest;
        _validatorEmailUpdateRequest = validatorEmailUpdateRequest;
        _validatorPasswordUpdateRequest = validatorPasswordUpdateRequest;
        _validatorUserNameUpdateRequest = validatorUserNameUpdateRequest;
        _validatorStatusUpdateRequest = validatorStatusUpdateRequest;
        _validatorDisplayStatusUpdateRequest = validatorDisplayStatusUpdateRequest;
        _validatorNotificationOptionsUpdateRequest = validatorNotificationOptionsUpdateRequest;

    }

    /// <summary>
    /// Updates the user's Profile Picture'
    /// </summary>
    /// <param name="request">
    /// A DTO containing the new Profile Picture'
    /// </param>
    /// <returns>
    /// Returns a JSON data as a response
    /// </returns>
    [HttpPut("update-profile-picture")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async Task<IActionResult> UpdateProfilePicture([FromBody] ProfilePictureUpdateRequestDTO request)
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
        var validationResult = _validatorProfilePictureRequest.Validate(request);

        if (!validationResult.IsValid)
        {
            return BadRequest("Invalid Profile Picture!");
        }

        // Update the Profile Picture of the account
        user.ProfilePicture = request.ProfilePictureFilename;
        var result = await _userManager.UpdateAsync(user);

        if (!result.Succeeded)
        {
            return BadRequest("There was a problem updating the Profile Picture!");
        }

        return Ok(new { message = "Profile Picture updated successfully!" });

    }

    [HttpPut("update-email")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async Task<IActionResult> UpdateEmail([FromBody] EmailUpdateRequestDTO request)
    {
        // Validate the request
        var validationResult = await _validatorEmailUpdateRequest.ValidateAsync(request);

        if (!validationResult.IsValid)
        {
            return ValidationProblem(new ValidationProblemDetails(validationResult.ToDictionary()));
        }

        // Get the currently logged-in user's email from JWT
        var userEmail = User.FindFirstValue(JwtRegisteredClaimNames.Email);
        var user = await _userManager.FindByEmailAsync(userEmail);

        // Check if the user exists
        if (user == null)
        {
            return NotFound(USER_NOT_FOUND);
        }

        // Safely update the Email
        var emailResult = await _userManager.SetEmailAsync(user, request.Email);

        if (!emailResult.Succeeded)
        {
            return BadRequest("There was a problem updating the Email!");
        }

        // Email Updated Successfully
        return Ok(new { message = "Email updated successfully!" });

    }

    /// <summary>
    /// Update the user's password'
    /// </summary>
    /// <param name="request">
    /// The DTO containing the new password
    /// </param>
    /// <returns>
    /// A JSON data as a response
    /// </returns>
    [HttpPut("update-password")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async Task<IActionResult> UpdatePassword([FromBody] PasswordUpdateRequestDTO request)
    {
        // Validate the Request
        var validatorResult = _validatorPasswordUpdateRequest.Validate(request);

        if (!validatorResult.IsValid)
        {
            return ValidationProblem(new ValidationProblemDetails(validatorResult.ToDictionary()));
        }

        // Get the currently logged-in user
        var userEmail = User.FindFirstValue(JwtRegisteredClaimNames.Email);
        var user = await _userManager.FindByEmailAsync(userEmail);

        // Check if the user exists
        if (user == null)
        {
            return NotFound(USER_NOT_FOUND);
        }

        // Try to change the password
        var result = await _userManager.ChangePasswordAsync(user, request.CurrentPassword, request.NewPassword);

        // There was a problem changing the password
        if (!result.Succeeded)
        {
            // Format the list of errors received from Identity
            foreach (var error in result.Errors)
            {
                // Adds to a ModelState (It integrates perfectly with ValidationProblem())
                ModelState.AddModelError("PasswordUpdate", error.Description);
            }

            // Returning the errors
            return ValidationProblem(ModelState);

        }

        // Password successfully updated
        return Ok(new { message = "Password successfully updated!" });

    }

    [HttpPut("update-username")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async Task<IActionResult> UpdateUserName(UserNameUpdateRequestDTO request)
    {

        // Validates the request
        var validationResult = await _validatorUserNameUpdateRequest.ValidateAsync(request);

        if (!validationResult.IsValid)
        {
            return ValidationProblem(new ValidationProblemDetails(validationResult.ToDictionary()));
        }

        // Get the currently logged-in user
        var userEmail = User.FindFirstValue(JwtRegisteredClaimNames.Email);
        var user = await _userManager.FindByEmailAsync(userEmail);

        // Check if the user exists
        if (user == null)
        {
            return NotFound(USER_NOT_FOUND);
        }

        // Try to update the UserName
        var result = await _userManager.SetUserNameAsync(user, request.UserName);

        if (!result.Succeeded)
        {
            return BadRequest("There was a problem updating the UserName!");
        }

        // UserName updated successfully
        return Ok(new { message = "UserName updated successfully!" });

    }

    [HttpPut("update-status")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async Task<IActionResult> UpdateStatus(StatusUpdateRequestDTO request)
    {

        // Validate the request
        var validationResult = await _validatorStatusUpdateRequest.ValidateAsync(request);

        if (!validationResult.IsValid)
        {
            return ValidationProblem(new ValidationProblemDetails(validationResult.ToDictionary()));
        }

        // Get the currently logged-in user
        var userEmail = User.FindFirstValue(JwtRegisteredClaimNames.Email);
        var user = await _userManager.FindByEmailAsync(userEmail);

        // Check if the user exists
        if (user == null)
        {
            return NotFound(USER_NOT_FOUND);
        }

        // Converting the string to the enum
        var newStatus = request.Status.ToEnum(UserStatus.Public);
        var newDisplayStatus = UserAccountUtils.ConvertToDisplayStatus(newStatus);

        // Try to update the User Status
        user.Status = newStatus;
        user.DisplayStatus = newDisplayStatus;
        var result = await _userManager.UpdateAsync(user);

        if (!result.Succeeded)
        {
            return BadRequest("There was a problem updating the User Status!");
        }

        // User Status updated successfully
        return Ok(new { message = "User Status updated successfully!" });

    }

    [HttpPut("update-display-status")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async Task<IActionResult> UpdateDisplayStatus(DisplayStatusUpdateRequestDTO request)
    {
        
        // Validate the request
        var validationResult = await _validatorDisplayStatusUpdateRequest.ValidateAsync(request);

        if (!validationResult.IsValid)
        {
            return ValidationProblem(new ValidationProblemDetails(validationResult.ToDictionary()));
        }
        
        // Get the currently logged-in user
        var userEmail = User.FindFirstValue(JwtRegisteredClaimNames.Email);
        var user = await _userManager.FindByEmailAsync(userEmail);

        // Check if the user exists
        if (user == null)
        {
            return NotFound(USER_NOT_FOUND);
        }
        
        // Verify if the DisplayStatus can be changed
        if (!user.Status.Equals(UserStatus.Public))
        {
            return ValidationProblem("The Display Status can only be changed for Public Users!");
        }
        
        // Try to update the Display Status
        var newDisplayStatus = request.DisplayStatus.ToEnum(UserStatus.Online);
        user.DisplayStatus = newDisplayStatus;
        var result = await _userManager.UpdateAsync(user);
        
        if (!result.Succeeded)
        {
            return BadRequest("There was a problem updating the Display Status!");
        }
        
        // Display Status updated successfully
        return Ok(new { message = "Display Status updated successfully!" });
        
    }

    [HttpPut("update-notification-options")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async Task<IActionResult> UpdateNotificationOptions(NotificationOptionsUpdateRequestDTO request)
    {
        
        // Validate the request
        var validationResult = await _validatorNotificationOptionsUpdateRequest.ValidateAsync(request);

        if (!validationResult.IsValid)
        {
            return ValidationProblem(new ValidationProblemDetails(validationResult.ToDictionary()));
        }
        
        // Get the currently logged-in user
        var userEmail = User.FindFirstValue(JwtRegisteredClaimNames.Email);
        var user = await _userManager.FindByEmailAsync(userEmail);
        
        // Check if the user exists
        if (user == null)
        {
            return NotFound(USER_NOT_FOUND);
        }
        
        // Convert the string to bool
        var notifyBasic = request.NotifyBasic;
        var notifyInvitations = request.NotifyInvitations;
        
        // Verify if the user already has these notification options 
        if (user.NotifyBasic.Equals(notifyBasic) && user.NotifyInvitations.Equals(notifyInvitations))
        {
            return Ok(new { message = "These notification options are already set!" });
        }
        
        // Try to update the notification options
        user.NotifyBasic = notifyBasic;
        user.NotifyInvitations = notifyInvitations;
        var result = await _userManager.UpdateAsync(user);
        
        if (!result.Succeeded)
        {
            return BadRequest("There was a problem updating the notification options!");
        }
        
        // Notification options updated successfully
        return Ok(new { message = "Notification options updated successfully!" });
        
    }

    /// <summary>
    /// A simple dashboard page to test the JWT 
    /// </summary>
    /// <returns>
    /// JSON data contaning the user's info
    /// </returns>
    [HttpGet("user-info")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [ProducesResponseType(typeof(DashboardResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<UserAccountInfoResponseDTO>> GetUserInfo()
    {
        
        // Get the currently logged-in user
        var userEmail = User.FindFirstValue(JwtRegisteredClaimNames.Email);
        var user = await _userManager.FindByEmailAsync(userEmail);

        // Check if the user exists
        if (user == null)
        {
            return NotFound(USER_NOT_FOUND);
        }    

        // Return user data
        return new UserAccountInfoResponseDTO{
            Username = user.UserName,
            Email = user.Email,
            ProfilePicture = user.ProfilePicture,
            Status = user.Status.ToString(),
            DisplayStatus = user.DisplayStatus.ToString(),
            AuthMethod = user.AuthenticationMethod.ToString(),
            NotifyBasic = user.NotifyBasic,
            NotifyInvitations = user.NotifyInvitations,
            };

    }
    

}