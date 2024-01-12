using SolarWatch.Model;

namespace SolarWatch.Service
{
    public interface ICityRepository
    {
        Task<string[]> GetCities();
        Task<City?> GetCityByNameAsync(string cityName, bool includeTwilights = false);

        Task AddCityAsync(City city);

        Task AddTwilightToCityAsync(City city, Twilight twilight);

        Task<int?> RemoveCityAsync(string cityName);

        Task<int?> UpdateTwilightDataAsync(City city);
    }
}
