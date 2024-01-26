using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace SolarWatch.Service.Authentication
{
    public class TokenService : ITokenService
    {
        private const int ExpirationMinutes = 30;
        private IConfiguration _configuration;

        public TokenService(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public string CreateToken(IdentityUser user, string role)
        {
            var expiration = DateTime.UtcNow.AddMinutes(ExpirationMinutes);
            var token = CreateJwtToken(CreateClaims(user, role), CreateSigningCredentials(), expiration);
            var tokenHandler = new JwtSecurityTokenHandler();
            return tokenHandler.WriteToken(token);
        }

        private JwtSecurityToken CreateJwtToken(List<Claim> claims, SigningCredentials credentials, DateTime expiration)
        {
            return new JwtSecurityToken("apiWithAuthBackend", "apiWithAuthBackend", claims, expires: expiration, signingCredentials: credentials);
        }

        private List<Claim> CreateClaims(IdentityUser user, string? role)
        {
            try
            {
                var claims = new List<Claim>
                {
                    new Claim(JwtRegisteredClaimNames.Sub, "TokenForTheApiWithAuth"),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.ToString(CultureInfo.InvariantCulture)),
                    new Claim(ClaimTypes.NameIdentifier, user.Id),
                    new Claim(ClaimTypes.Name, user.UserName),
                    new Claim(ClaimTypes.Email, user.Email)
                };
                if(role != null)
                {
                    claims.Add(new Claim(ClaimTypes.Role, role));
                }
                return claims;
            }catch(Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }

        private SigningCredentials CreateSigningCredentials()
        {
            return new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["IssuerSigningKey"])), SecurityAlgorithms.HmacSha256);
        }
    }
}
