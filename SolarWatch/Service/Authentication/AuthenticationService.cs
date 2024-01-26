using Microsoft.AspNetCore.Identity;
using SolarWatch.Contracts;
using System.IdentityModel.Tokens.Jwt;
using System.IO;

namespace SolarWatch.Service.Authentication
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ITokenService _tokenService;

        public AuthenticationService(UserManager<IdentityUser> userManager, ITokenService tokenService)
        {
            _userManager = userManager;
            _tokenService = tokenService;
        }

        public async Task<AuthenticationResult> LoginAsync(string email, string password)
        {
            var managedUser = await _userManager.FindByEmailAsync(email);
            if(managedUser == null)
            {
                return InvalidEmail(email);
            }
            
            var isPasswordValid = await _userManager.CheckPasswordAsync(managedUser, password);
            if (!isPasswordValid)
            {
                return InvalidPassword(email, managedUser.UserName);
            }

            var roles = await _userManager.GetRolesAsync(managedUser);
            var accessToken = _tokenService.CreateToken(managedUser, roles[0]);
            return new AuthenticationResult(true, managedUser.Email, managedUser.UserName, accessToken);
        }

        private static AuthenticationResult InvalidEmail(string email)
        {
            var result = new AuthenticationResult(false, email,"","");
            result.ErrorMessages.Add("Bad credentials", "Invalid email");
            return result;
        }
        
        private static AuthenticationResult InvalidPassword(string email, string username)
        {
            var result = new AuthenticationResult(false, email, username, "");
            result.ErrorMessages.Add("Bad credentials", "Invalid password");
            return result;
        }

        public async Task<AuthenticationResult> RegisterAsync(string email, string username, string password, string role)
        {
            var user = new IdentityUser { UserName = username, Email = email };
            var result = await _userManager.CreateAsync(user, password);
            if(!result.Succeeded)
            {
                return FailedRegistration(result, email, username);
            }
            await _userManager.AddToRoleAsync(user, role);
            return new AuthenticationResult(true, email, username, "");
        }

        private static AuthenticationResult FailedRegistration(IdentityResult result, string email, string username)
        {
            var authenticationResult = new AuthenticationResult(false, email, username, "");
            foreach(var error in  result.Errors)
            {
                authenticationResult.ErrorMessages.Add(error.Code, error.Description);
            }
            return authenticationResult;
        }

        public async Task<AuthenticationResult> CheckTokenValidityAsync(string tokenAsString)
        {
            var handler = new JwtSecurityTokenHandler();
            var jsonToken = handler.ReadToken(tokenAsString);
            var decodedJwtToken = jsonToken as JwtSecurityToken;

            if(decodedJwtToken == null)
            {
                var result = new AuthenticationResult(false, "", "", tokenAsString);
                result.ErrorMessages.Add("Invalid token!", "Invlaid token format!");
                return result;
            }

            var emailClaim = decodedJwtToken?.Claims.First(claim => claim.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress").Value;
            var expiryClaim = decodedJwtToken?.Claims.First(claim => claim.Type == "exp").Value;
            
            IdentityUser? userWithToken = await _userManager.FindByEmailAsync(emailClaim);

            if(userWithToken == null)
            {
                var result = new AuthenticationResult(false, "", "", tokenAsString);
                result.ErrorMessages.Add("Invalid token!", "User not found!");
                return result;
            }
            if (expiryClaim == null || !long.TryParse(expiryClaim, out var expirationTime) || DateTimeOffset.FromUnixTimeSeconds(expirationTime) < DateTimeOffset.UtcNow)
            {
                var result = new AuthenticationResult(false, userWithToken.Email, userWithToken.UserName, tokenAsString);
                result.ErrorMessages.Add("Invalid token!", "Token expired, new login required!");
                return result;
            }

            return new AuthenticationResult(true, userWithToken.Email, userWithToken.UserName, tokenAsString);
        }
    }
}
