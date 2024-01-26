using SolarWatch.Contracts;

namespace SolarWatch.Service.Authentication
{
    public interface IAuthenticationService
    {
        Task<AuthenticationResult> RegisterAsync(string email, string username, string password, string role);
        Task<AuthenticationResult> LoginAsync(string username, string password);
        Task<AuthenticationResult> CheckTokenValidityAsync(string tokenAsString);
    }
}
