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
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _configuration;

        public JWTService(SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager, IConfiguration configuration)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _configuration = configuration;
        }

        // Used ONLY by Email/Password Login
        public async Task<LoginResponseDTO?> Authenticate(LoginRequestDTO request)
        {
            // Get user
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null) return null;

            var result = await _signInManager.PasswordSignInAsync(user.UserName, request.Password, isPersistent: false, lockoutOnFailure: false);

            if (result.Succeeded)
            {
                // If password is correct, generate token 
                return GenerateToken(user);
            }

            return null;
        }

        // Generating the token
        public LoginResponseDTO GenerateToken(ApplicationUser user)
        {
            var issuer = _configuration["JwtConfig:Issuer"];
            var audience = _configuration["JwtConfig:Audience"];
            var secret = _configuration["JwtConfig:Secret"];
            var tokenValidityMins = _configuration.GetValue<int>("JwtConfig:ExpirationInMinutes");
            var tokenExpiryTimeStamp = DateTime.UtcNow.AddMinutes(tokenValidityMins);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[] {
                    new Claim(JwtRegisteredClaimNames.Name, user.Email),
                    new Claim(JwtRegisteredClaimNames.GivenName, user.DisplayName ?? "")
                }),
                Expires = tokenExpiryTimeStamp,
                Issuer = issuer,
                Audience = audience,

                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret)),
                        SecurityAlgorithms.HmacSha512Signature),
            };

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