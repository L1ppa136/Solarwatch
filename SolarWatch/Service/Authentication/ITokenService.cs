using Microsoft.AspNetCore.Identity;

namespace SolarWatch.Service.Authentication
{
    public interface ITokenService
    {
        string CreateToken(IdentityUser user, string role);
    }
}
