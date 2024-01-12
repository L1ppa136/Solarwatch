namespace SolarWatch.Contracts
{    
    public record AuthenticationResult(
        bool Success,
        string Email,
        string UserName,
        string Token)
    {
        //Error code - error message
        public readonly Dictionary<string, string> ErrorMessages = new();
    }
}
