using SolarWatch.Model;

namespace SolarWatch.Service
{
    public interface ITwilightDataFetcher
    {
        Task<Twilight> GetTwilightDataAsync(string url);
    }
}
