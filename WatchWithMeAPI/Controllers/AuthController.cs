using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Security.Claims;
using WatchWithMeAPI.DTO;
using WatchWithMeAPI.Model;

[Route("api/auth")]
[ApiController]
public class AuthController : ControllerBase
{

    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly UserManager<ApplicationUser> _userManager;

    // Injecting Identity managers provided by ASP.NET Core
    public AuthController(SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager)
    {
        _signInManager = signInManager;
        _userManager = userManager;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDTO model) {

        // 1. Validate the incoming data
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        // 2. Find the user by their email
        var user = await _userManager.FindByEmailAsync(model.Email);
        if (user == null)
        {
            // Security Best Practice: Don't tell the user "Email not found"
            // Always use a generic message so hackers can't guess valid emails
            return Unauthorized(new { message = "Invalid email or password." });
        }

        // 3. Attempt to sign in
        // Parameters: UserName, Password, RememberMe (isPersistent), LockoutOnFailure
        var result = await _signInManager.PasswordSignInAsync(user.UserName, model.Password, isPersistent: false, lockoutOnFailure: false);

        if (result.Succeeded)
        {
            // The password matched the database!
            // LATER: This is EXACTLY where we will generate and return the JWT.

            return Ok(new
            {
                message = "Login successful!",
                displayName = user.DisplayName,
                email = user.Email
            });
        }

        // 4. If the password was wrong
        return Unauthorized(new { message = "Invalid email or password." });

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

        // Attempt to sing in the user if they've already linked this Google account before
        // This checks the AspNetUserLogins table
        var signInResult = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: false, bypassTwoFactor: true);

        if (signInResult.Succeeded) {

            // User exists and is linked
            // Redirect back to frontend
            // JWT Token will be added here
            return Redirect("http://localhost:4200/login-success?msg=login");
        }

        // If the sing-in failed, it means this is a new user, or a user who hasn't linked Google yet
        var email = info.Principal.FindFirstValue(ClaimTypes.Email);
        var username = info.Principal.FindFirstValue(ClaimTypes.GivenName);
        if (username == null)
        {
            username = info.Principal.FindFirstValue(ClaimTypes.Name);
        }

        if (email != null && username != null) {


            // Check if a user with this email already exists in the AspNetUsers table
            var user = await _userManager.FindByEmailAsync(email);

            // User does not exists, so we will register him
            if (user == null) {


                user = new ApplicationUser { UserName = email, Email = email, DisplayName = username };
                var createResult = await _userManager.CreateAsync(user);

                if (!createResult.Succeeded) {

                    // ADD THIS: Print the exact errors to your backend console
                    foreach (var error in createResult.Errors)
                    {
                        Console.WriteLine($"\n=== IDENTITY ERROR ===");
                        Console.WriteLine($"Code: {error.Code}");
                        Console.WriteLine($"Description: {error.Description}\n");
                    }

                    return Redirect("http://localhost:4200/login?error=registration_failed");
                }

            }

            // Link this Google account to the user in the AspNetUserLogins table
            var linkResult = await _userManager.AddLoginAsync(user, info);

            if (linkResult.Succeeded) {
                
                // Sing in the user in
                await _signInManager.SignInAsync(user, isPersistent: false);

                // Redirect to frontend
                return Redirect("http://localhost:4200/login-success?msg=register");

            }

        }

        // Fallback if something goes wrong
        return Redirect("http://localhost:4200/login?error=unknown_error");

    }



}