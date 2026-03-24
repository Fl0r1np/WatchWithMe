using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using WatchWithMeAPI.DTO;
using WatchWithMeAPI.Model;
using WatchWithMeAPI.Services;

[Route("api/auth")]
[ApiController]
public class AuthController : ControllerBase
{
    // Necessary services
    private readonly SignInManager<User> _signInManager;
    private readonly UserManager<User> _userManager;
    private readonly JWTService _jwtService;

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
    public AuthController(SignInManager<User> signInManager, UserManager<User> userManager, JWTService jwtService
        )
    {
        _signInManager = signInManager;
        _userManager = userManager;
        _jwtService = jwtService;
    }

    /// <summary>
    /// Handles the basic login request
    /// </summary>
    /// <param name="loginRequest">
    /// DTO containing the login data
    /// </param>
    /// <returns>
    /// Returns JSON data as a response
    /// </returns>
    [HttpPost("login")]
    public async Task<ActionResult<LoginResponseDTO>> Login([FromBody] LoginRequestDTO loginRequest) {

        // Validate the incoming data
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        // Getting the login response based on the login request
        var loginResponse = await _jwtService.Authenticate(loginRequest);

        // The credentials are invalid
        if (loginResponse is null)
        {
            return Unauthorized(new { 
                message = "Invalid email or password." 
            });

        }

        // Credentials are valid
        return loginResponse; 


    }

    /// <summary>
    /// Handles the basic registration request
    /// </summary>
    /// <param name="registerRequest">
    /// DTO containing the user data
    /// </param>
    /// <returns>
    /// Returns a JSON data as a response
    /// </returns>
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterDTO registerRequest) {

        // Check if the incoming data passed the validation rules in the DTO
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        // Create the new user object
        var user = new User
        {
            UserName = registerRequest.Username,
            Email = registerRequest.Email,
        };
        
        

        // Save the user and hash their password in one step
        var result = await _userManager.CreateAsync(user, registerRequest.Password);

        if (result.Succeeded)
        {
            // Return a 200 response's code to Front-End
            return Ok(new { message = "User registered successfully!" });
        }

        // If it failed (e.g., password too weak, email already taken), return the errors
        return BadRequest(result.Errors);

    }

    /// <summary>
    /// Handles the login with a Google Account
    /// </summary>
    /// <param name="provider">
    /// What provider to use for login
    /// </param>
    /// <returns>
    /// 
    /// </returns>
    [HttpGet("login-google")]
    public IActionResult LoginGoogle(string provider = "Google")
    {
        // Method to call when a succes login happens
        var redirectUrl = Url.Action(nameof(GoogleCallback), "Auth");

        // Preparing the set of metadata for external login for provider
        var proprieties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);

        // Redirecting the Front-End to the provider
        return Challenge(proprieties, provider);

    }

    /// <summary>
    /// Responsible for identifying the user, creating their account if they’re new, and finally handing them a JWT so they can actually use the app
    /// </summary>
    /// <returns>
    /// Returns a Redirect to Front-End Handler page
    /// </returns>
    [HttpGet("google-callback")]
    public async Task<IActionResult> GoogleCallback()
    {
        
        // Get the information Google sent back about the user
        var info = await _signInManager.GetExternalLoginInfoAsync();

        // Google Authentification failed
        if (info == null)
        {
            return Redirect("http://localhost:4200/login?error=google_auth_failed");
        }

        // Attempt to sign in the user if they've already linked this Google account before
        var signInResult = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: false, bypassTwoFactor: true);

        User user = null;

        if (signInResult.Succeeded) // Existing user
        {
            
            // Fetch the user object from the database using their Google ID so we can generate a token
            user = await _userManager.FindByLoginAsync(info.LoginProvider, info.ProviderKey);

        }
        else { // New user

            // Getting user data
            var googleEmail = info.Principal.FindFirstValue(ClaimTypes.Email);
            var googleName = info.Principal.FindFirstValue(ClaimTypes.Name) ?? "Username";
            
            // Strip out invalid characters from the username
            var sanitizedName = new string(googleName.Where(char.IsLetterOrDigit).ToArray());
            
            // Append a short random string to the end of the username to avoid any potential collisions
            var uniqueSuffix = Guid.NewGuid().ToString().Substring(0, 6);
            var finalUserName = $"{sanitizedName}{uniqueSuffix}";
            
            
            
            if (googleEmail != null && finalUserName != null)
            {


                // Check if a user with this email already exists
                user = await _userManager.FindByEmailAsync(googleEmail);

                // User does not exist, so we will register him
                if (user == null)
                {
 
                    user = new User { UserName = finalUserName, Email = googleEmail };
                    var createResult = await _userManager.CreateAsync(user);

                    // There was a problem registering the user
                    if (!createResult.Succeeded)
                    {
                        return Redirect("http://localhost:4200/login?error=registration_failed");
                    }

                }

                // Link this Google account to the user in the AspNetUserLogins table
                var linkResult = await _userManager.AddLoginAsync(user, info);

                // There was an unknown error
                if (!linkResult.Succeeded)
                {

                    return Redirect("http://localhost:4200/login?error=unkown_error");

                }

            }

        }

        // Generate the JWT and Redirect
        if (user != null) {

            // Getting the JWT
            var jwtResponse = _jwtService.GenerateToken(user);

            // Redirect
            return Redirect($"http://localhost:4200/login-success?token={jwtResponse.AccessToken}");

        }

        // Fallback
        return Redirect("http://localhost:4200/login?error=unknown_error");

    }

    /// <summary>
    /// A simple dashboard page to test the JWT 
    /// </summary>
    /// <returns>
    /// JSON data contaning the user's info
    /// </returns>
    [HttpGet("dashboard")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public IActionResult GetDashboardData(){

        var userEmail = User.FindFirstValue(JwtRegisteredClaimNames.Email) ?? "Email";
        var userName = User.FindFirstValue(JwtRegisteredClaimNames.GivenName) ?? "Username";

        return Ok( 
            new {
                message = $"Welcome to the Dashboard, {userName}!",
                email = userEmail,
                secretData = "Here is the private data only logged-in users can see."

        });

    }

}