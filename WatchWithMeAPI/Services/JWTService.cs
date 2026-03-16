using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using WatchWithMeAPI.DTO;
using WatchWithMeAPI.Model;

namespace WatchWithMeAPI.Services
{
    public class JWTService
    {
        
        // Necessary services
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
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
        /// <param name="configuration">
        /// An interface provided by .NET that gives you a unified view of all your settings
        /// </param>
        public JWTService(SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager, IConfiguration configuration)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _configuration = configuration;
        }

        /// <summary>
        /// Method that handles the basic authentication of the user ( email & password )
        /// </summary>
        /// <param name="loginRequest">
        /// A LoginRequestDTO containing the user's credentials
        /// </param>
        /// <returns>
        /// Returns a JWT token generated with GenerateToken method
        /// </returns>
        public async Task<LoginResponseDTO?> Authenticate(LoginRequestDTO loginRequest)
        {
            // Get the user
            var user = await _userManager.FindByEmailAsync(loginRequest.Email);
            if (user == null) return null;

            // Trying to sign in the user
            var result = await _signInManager.PasswordSignInAsync(user.UserName, loginRequest.Password, isPersistent: false, lockoutOnFailure: false);

            // If password is correct, generate JWT token 
            if (result.Succeeded) 
            {
                return GenerateToken(user);
            }

            // Login failed
            return null;
        }

        /// <summary>
        /// Method that generates a JWT token for a specific user 
        /// </summary>
        /// <param name="user">
        /// The user for whom the token is generated
        /// </param>
        /// <returns>
        /// Returns an LoginResponseDTO containing the JWT token and additional info
        /// </returns>
        public LoginResponseDTO GenerateToken(ApplicationUser user)
        {
            
            // Get the necessary data from the configuration
            var issuer = _configuration["JwtConfig:Issuer"];
            var audience = _configuration["JwtConfig:Audience"];
            var secret = _configuration["JwtConfig:Secret"];
            var tokenValidityMins = _configuration.GetValue<int>("JwtConfig:ExpirationInMinutes");
            var tokenExpiryTimeStamp = DateTime.UtcNow.AddMinutes(tokenValidityMins);

            // Create the token's descriptor
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[] {
                    new Claim(JwtRegisteredClaimNames.Name, user.Email),
                    new Claim(JwtRegisteredClaimNames.GivenName, user.DisplayName ?? "")
                }),
                Expires = tokenExpiryTimeStamp,
                Issuer = issuer,
                Audience = audience,

                // Creates a digital signature
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret)),
                        SecurityAlgorithms.HmacSha512Signature),
            };

            // Create the actual token and return it
            var tokenHandler = new JwtSecurityTokenHandler();
            var securityToken = tokenHandler.CreateToken(tokenDescriptor);
            var accessToken = tokenHandler.WriteToken(securityToken);

            return new LoginResponseDTO
            {
                AccessToken = accessToken
            };
        }
    }
}