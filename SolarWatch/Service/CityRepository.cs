using Microsoft.EntityFrameworkCore;
using SolarWatch.Data;
using SolarWatch.Model;
using System.Text;

namespace SolarWatch.Service
{
    public class CityRepository : ICityRepository
    {
        private readonly SolarwatchDbContext _dbContext;

        public CityRepository(SolarwatchDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<string[]> GetCities()
        {
            List<City> cities = await _dbContext.Cities.ToListAsync();
            List<string> cityNames = new List<string>();
            foreach(City city in cities) 
            {
                cityNames.Add(city.Name);
            };
            return cityNames.ToArray();
        }

        public async Task AddCityAsync(City city)
        {
            _dbContext.Cities.Add(city);
            await _dbContext.SaveChangesAsync();
        }

        public async Task AddTwilightToCityAsync(City city, Twilight twilight)
        {
            City? cityToUpdate = await GetCityByNameAsync(city.Name);
            if (cityToUpdate == null)
            {
                throw new ArgumentNullException(nameof(city));
            }
            cityToUpdate.AddTwilightData(twilight);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<City?> GetCityByNameAsync(string cityName, bool includeTwilights = false)
        {
            var query =  _dbContext.Cities.AsQueryable();

            if (includeTwilights)
            {
                query = query.Include(c => c.Twilights);
            }

            return await query.FirstOrDefaultAsync(c => c.Name == cityName);
        }

        public async Task<int?> RemoveCityAsync(string cityName)
        {
            City? cityToBeRemoved = await GetCityByNameAsync(cityName);
            if (cityToBeRemoved == null)
            {
                return null;
            }
            _dbContext.Cities.Remove(cityToBeRemoved);
            await _dbContext.SaveChangesAsync();
            return cityToBeRemoved.CityId;
        }

        public async Task<int?> UpdateTwilightDataAsync(City city)
        {
            _dbContext.Cities.Update(city);
            await _dbContext.SaveChangesAsync();
            return city.CityId;
        }
    }
}
