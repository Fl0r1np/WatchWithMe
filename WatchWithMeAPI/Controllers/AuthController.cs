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
    // Services
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly JWTService _jwtService;

    public AuthController(SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager, JWTService jwtService
        )
    {
        _signInManager = signInManager;
        _userManager = userManager;
        _jwtService = jwtService;
    }

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
            return Unauthorized(new { message = "Invalid email or password." });

        }

        // Credentials are valid
        return loginResponse; 


    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterDTO model) {

        // 1. Check if the incoming data passed the validation rules in the DTO
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        // 2. Create the new user object
        var user = new ApplicationUser
        {
            UserName = model.Email,
            Email = model.Email,
            DisplayName = model.Username
        };

        // 3. Save the user AND hash their password in one step
        var result = await _userManager.CreateAsync(user, model.Password);

        if (result.Succeeded)
        {
            // Success! Return a 200 OK status to Angular
            return Ok(new { message = "User registered successfully!" });
        }

        // 4. If it failed (e.g., password too weak, email already taken), return the errors
        return BadRequest(result.Errors);

    }

    [HttpGet("login-google")]
    public IActionResult LoginGoogle(string provider = "Google")
    {

        var redirectUrl = Url.Action(nameof(GoogleCallback), "Auth");

        var proprieties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);

        return Challenge(proprieties, provider);

    }

    [HttpGet("google-callback")]
    public async Task<IActionResult> GoogleCallback()
    {
        
        // Get the information Google sent back about the user
        var info = await _signInManager.GetExternalLoginInfoAsync();

        if (info == null)
        {
            return Redirect("http://localhost:4200/login?error=google_auth_failed");
        }

        // Attempt to sign in the user if they've already linked this Google account before
        var signInResult = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: false, bypassTwoFactor: true);

        ApplicationUser user = null;

        if (signInResult.Succeeded) // Existing user
        {
            
            // Fetch the user object from the database using thei Google ID so we can generate a token
            user = await _userManager.FindByLoginAsync(info.LoginProvider, info.ProviderKey);

        }
        else { // New user

            // Getting user data
            var email = info.Principal.FindFirstValue(ClaimTypes.Email);
            var username = info.Principal.FindFirstValue(ClaimTypes.GivenName) ?? info.Principal.FindFirstValue(ClaimTypes.Name);

            if (email != null && username != null)
            {


                // Check if a user with this email already exists
                user = await _userManager.FindByEmailAsync(email);

                // User does not exists, so we will register him
                if (user == null)
                {

                    user = new ApplicationUser { UserName = email, Email = email, DisplayName = username };
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

    [HttpGet("dashboard")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public IActionResult GetDashboardData(){

        var userEmail = User.FindFirstValue(JwtRegisteredClaimNames.Name) ?? "Email";
        var userName = User.FindFirstValue(JwtRegisteredClaimNames.GivenName) ?? "Username";

        return Ok( 
            new {
                message = $"Welcome to the Dashboard, {userName}!",
                email = userEmail,
                secretData = "Here is the private data only logged-in users can see."

        });

    }

}