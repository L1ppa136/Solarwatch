using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SolarWatch.Contracts;
using SolarWatch.Model;
using SolarWatch.Service;
using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace SolarWatch.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TwilightController : ControllerBase
    {
        private readonly ILogger<TwilightController> _logger;
        private readonly IGeocoder _geocoder;
        private readonly ITwilightDataFetcher _twilightDataFetcher;
        private readonly ICityRepository _cityRepository;
        private readonly IWeatherDescriptionProvider _weatherDescriptionProvider;

        public TwilightController(ILogger<TwilightController> logger, IGeocoder geocoder, ITwilightDataFetcher twilightDataFetcher, ICityRepository cityRepository, IWeatherDescriptionProvider weatherDescriptionProvider)
        {
            _logger = logger;
            _geocoder = geocoder;
            _twilightDataFetcher = twilightDataFetcher;
            _cityRepository = cityRepository;
            _weatherDescriptionProvider = weatherDescriptionProvider;   
        }

        [HttpGet("Cities")]
        public async Task<IActionResult> GetAllCities()
        {
            string[] cities = await _cityRepository.GetCities();
            return Ok(cities);
        }

        [HttpPost("Solarwatch"), Authorize]
        public async Task<IActionResult> Get([FromBody]SolarWatchRequestDTO request)
        {
            try
            {
                Geocode geocode;
                Twilight? twilightData;
                string newDate = "";
                if (IsValidDateFormat(request.Date) && request.Date != null)
                {
                    newDate = request.Date;
                }
                else if(request.Date == null)
                {
                    newDate = "today";
                }
                else
                {
                    return BadRequest("Something went wrong - Invalid date format");
                }


                City? cityFromDB = await _cityRepository.GetCityByNameAsync(request.CityName, includeTwilights: true);
                string? weatherDescription;
                if (cityFromDB != null)
                {
                    geocode = new Geocode(cityFromDB.Latitude, cityFromDB.Longitude, cityFromDB.Country, cityFromDB.State);
                    twilightData = cityFromDB.GetTwilightByDate(newDate);
                    weatherDescription = await _weatherDescriptionProvider.ProvideWeatherDescription(geocode);

                    if (twilightData != null)
                    {
                        twilightData.AddWeatherDescription(weatherDescription);
                        return Ok(twilightData);
                    }

                }
                else
                {
                    geocode = await _geocoder.GetGeoCodesAsync(request.CityName);
                }
               
                twilightData = await FetchTwilightData(geocode, newDate);

                if (cityFromDB != null)
                {
                    await AddNewTwilightToExistingCityAsync(cityFromDB, twilightData);
                }
                else
                {
                    await NewCityAndTwilightToDBAsync(request.CityName, geocode, twilightData);
                }
                weatherDescription = await _weatherDescriptionProvider.ProvideWeatherDescription(geocode);
                twilightData.AddWeatherDescription(weatherDescription);

                return Ok(twilightData);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                return BadRequest($"Something went wrong - {ex.Message}");
            }
        }

        private bool IsValidDateFormat(string dateString)
        {
            string dateFormat = "yyyy-MM-dd";

            return DateTime.TryParseExact(dateString, dateFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out _);
        }

        private async Task<Twilight> FetchTwilightData(Geocode geocode, string date)
        {
            string urlWithDate = $"https://api.sunrise-sunset.org/json?lat={geocode.Latitude}&lng={geocode.Longitude}&date={date}";
            Twilight twilightData = await _twilightDataFetcher.GetTwilightDataAsync(urlWithDate);
            twilightData.SetDate(date);
            return twilightData;
        }

        private async Task AddNewTwilightToExistingCityAsync(City cityFromDB, Twilight twilightData)
        {
            await _cityRepository.AddTwilightToCityAsync(cityFromDB, twilightData);
        }

        private async Task NewCityAndTwilightToDBAsync(string cityName, Geocode geocode, Twilight twilightData)
        {
            City newCity = new City(cityName, geocode.Latitude, geocode.Longitude, geocode.State, geocode.Country);
            await _cityRepository.AddCityAsync(newCity);
            await _cityRepository.AddTwilightToCityAsync(newCity, twilightData);
        }

        [HttpPatch("updateTwilightData"), Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateCityAsync(CityUpdateRequest request)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Twilight newTwilightData = new Twilight(request.Sunrise, request.Sunset, request.SolarNoon);
            newTwilightData.SetDate(request.Date);
            City? city = await _cityRepository.GetCityByNameAsync(request.CityName, includeTwilights: true);
            
            if (city == null)
            {
                return NotFound($"City could not be found by this name: {request.CityName}");
            }
            
            city.AddTwilightData(newTwilightData);


            try
            {
                int? updatedCityId = await _cityRepository.UpdateTwilightDataAsync(city);

                if (updatedCityId != null)
                {
                    return Ok(updatedCityId);
                }

                return StatusCode(StatusCodes.Status500InternalServerError, "Failed to update city");
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while updating city");
            }
        }
    }
}

