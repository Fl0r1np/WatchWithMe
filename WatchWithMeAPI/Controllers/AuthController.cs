using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using WatchWithMeAPI.DTO;
using WatchWithMeAPI.Model;
using WatchWithMeAPI.ResponseRecords;
using WatchWithMeAPI.Services;

namespace Controllers
{
    
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        // Necessary services
        private readonly SignInManager<User> _signInManager;
        private readonly UserManager<User> _userManager;
        private readonly JWTService _jwtService;

        // Config
        private readonly IConfiguration _configuration;

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
        public AuthController(SignInManager<User> signInManager,
            UserManager<User> userManager,
            JWTService jwtService,
            IConfiguration configuration
        )
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _jwtService = jwtService;
            _configuration = configuration;
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
        public async Task<ActionResult<LoginResponseDTO>> Login([FromBody] LoginRequestDTO loginRequest)
        {

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
                return Unauthorized(new
                {
                    message = "Invalid email or password."
                });

            }

            // Credentials are valid
            return loginResponse;


        }

        /// <summary>
        /// Handles the basic registration request
        /// </summary>
        /// <param name="registerRequestRequest">
        /// DTO containing the user data
        /// </param>
        /// <returns>
        /// Returns a JSON data as a response
        /// </returns>
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequestDTO registerRequestRequest)
        {

            // Check if the incoming data passed the validation rules in the DTO
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Create the new user object
            var user = new User
            {
                UserName = registerRequestRequest.Username,
                Email = registerRequestRequest.Email,
            };



            // Save the user and hash their password in one step
            var result = await _userManager.CreateAsync(user, registerRequestRequest.Password);

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
                return GenerateErrorRedirect("google_auth_failed");
            }

            // Attempt to sign in the user if they've already linked this Google account before
            var signInResult = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey,
                isPersistent: false, bypassTwoFactor: true);

            // User already exists
            if (signInResult.Succeeded)
            {
                var existingUser = await _userManager.FindByLoginAsync(info.LoginProvider, info.ProviderKey);
                return GenerateSuccessRedirect(existingUser);
            }

            // New user
            var googleEmail = info.Principal.FindFirstValue(ClaimTypes.Email);
            if (string.IsNullOrEmpty(googleEmail))
            {
                return GenerateErrorRedirect("missing_email");
            }

            // Check if a user with this email already exists
            var user = await _userManager.FindByEmailAsync(googleEmail);

            if (user == null)
            {
                // Gen the Google username and sanitize it
                var googleName = info.Principal.FindFirstValue(ClaimTypes.Name) ?? "Username";
                var sanitizedName = new string(googleName.Where(char.IsLetterOrDigit).ToArray());
                var uniqueSuffix = Guid.NewGuid().ToString().Substring(0, 6);

                // Create the user
                user = new User { UserName = $"{sanitizedName}{uniqueSuffix}", Email = googleEmail };
                var createResult = await _userManager.CreateAsync(user);

                if (!createResult.Succeeded)
                {
                    return GenerateErrorRedirect("register_failed");
                }
            }

            // Link the Google account to the user
            var linkResult = await _userManager.AddLoginAsync(user, info);
            if (!linkResult.Succeeded)
            {
                return GenerateErrorRedirect();
            }

            // Generate JWT and Redirect
            return GenerateSuccessRedirect(user);

        }

        /// <summary>
        /// Generates a Success Redirect to the Front-End Handler page
        /// </summary>
        /// <param name="user">
        /// The user that just logged in
        /// </param>
        /// <returns>
        /// Returns a Redirect to Front-End Handler page
        /// </returns>
        private IActionResult GenerateSuccessRedirect(User user)
        {
            // If the user is null, something went wrong
            if (user == null)
            {
                return GenerateErrorRedirect();
            }

            // Generate JWT
            var jwtResponse = _jwtService.GenerateToken(user, AuthMethod.Google);
            var frontendUrl = _configuration["FrontendSettings:BaseUrl"];
            return Redirect($"{frontendUrl}/login-success?token={jwtResponse.AccessToken}");
        }

        /// <summary>
        /// Generates a Error Redirect to the Front-End Handler page
        /// </summary>
        /// <param name="error">
        /// A string containing the error message
        /// </param>
        /// <returns>
        /// A redirect to Front-End Handler page
        /// </returns>
        private IActionResult GenerateErrorRedirect(string error = "unkown_error")
        {
            var frontendUrl = _configuration["FrontendSettings:BaseUrl"];
            return Redirect($"{frontendUrl}/login?error={error}");
        }

    }
}