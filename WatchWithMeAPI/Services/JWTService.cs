using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
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
        private readonly WatchWithMeContext _dbContext;
        private readonly IConfiguration _configuration;

        public JWTService(SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager, WatchWithMeContext dbContext, IConfiguration configuration  ) {
            _signInManager = signInManager;
            _userManager = userManager;
            _dbContext = dbContext;
            _configuration = configuration;
        }


        public async Task<LoginResponseDTO?> Authenticate(LoginRequestDTO request) {


            // Look for the user
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null)
            { 
                return null;
            }

            // Attempt to sign in
            var result = await _signInManager.PasswordSignInAsync(user.UserName, request.Password, isPersistent: false, lockoutOnFailure: false);

            if (result.Succeeded)
            {

                // Getting necessary data
                var issuer = _configuration["JwtConfig:Issuer"];
                var audience = _configuration["JwtConfig:Audience"];
                var secret = _configuration["JwtConfig:Secret"];
                var tokenValidityMins = _configuration.GetValue<int>("JwtConfig:ExpirationInMinutes");
                var tokenExpiryTimeStamp = DateTime.UtcNow.AddMinutes(tokenValidityMins);

                // Creating the token
                var tokenDescriptor = new SecurityTokenDescriptor
                {

                    Subject = new ClaimsIdentity(
                            new[] {
                                new Claim(JwtRegisteredClaimNames.Name, request.Email)
                            }
                        ),
                    Expires = tokenExpiryTimeStamp,
                    Issuer = issuer,
                    Audience = audience,
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.ASCII.GetBytes(secret)),
                            SecurityAlgorithms.HmacSha512Signature),

                };

                var tokenHandler = new JwtSecurityTokenHandler();
                var securityToken = tokenHandler.CreateToken(tokenDescriptor);
                var accessToken = tokenHandler.WriteToken(securityToken);


                // The password matched the database
                return new LoginResponseDTO
                {
                    Username = user.DisplayName,
                    Email = user.Email,
                    AccessToken = accessToken,
                    ExpiresIn = (int)tokenExpiryTimeStamp.Subtract(DateTime.UtcNow).TotalSeconds

                };
            }


            // If there was a problem checking the credentials
            return null;



        }

    }
}
