using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using WebManagementApp.DataAccess.Interfaces;
using WebManagementApp.Models;

namespace WebManagementApp.DataAccess.Services
{
    public class TokenService:ITokenService
    {
        //private const double EXPIRY_DURATION_MINUTES = 360 ;
        private readonly double EXPIRY_DURATION_MINUTES;

        public TokenService(IConfiguration configuration)
        {
            EXPIRY_DURATION_MINUTES = configuration.GetValue<double>("Jwt:ExpiryMinutes");
        }

        public string BuildToken(string key, string issuer, UserDTO user)
        {
            //create claims details based on the user information
            var claims = new[] {
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Name, user.Name),
                new Claim(ClaimTypes.NameIdentifier,user.idUser.ToString()),
                new Claim(ClaimTypes.Role, user.UserRole) // Ensure this is set
            };
            
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature);
            var tokenDescriptor = new JwtSecurityToken(issuer, issuer, claims,
                expires: DateTime.Now.AddMinutes(EXPIRY_DURATION_MINUTES), signingCredentials: credentials);
            return new JwtSecurityTokenHandler().WriteToken(tokenDescriptor);
        }

        // To validate token
        public bool IsTokenValid(string key, string issuer, string token)
        {
            var mySecret = Encoding.UTF8.GetBytes(key);
            var mySecurityKey = new SymmetricSecurityKey(mySecret);
            var tokenHandler = new JwtSecurityTokenHandler();
            try
            {
                tokenHandler.ValidateToken(token,
                new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidIssuer = issuer,
                    ValidAudience = issuer,
                    IssuerSigningKey = mySecurityKey,
                }, out SecurityToken validatedToken);
            }
            catch
            {
                return false;
            }
            return true;
        }
    }
}
