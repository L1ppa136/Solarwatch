using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.OpenApi.Validations;
using SolarWatch.Controllers;
using SolarWatch.Model;
using System.Net;
using System.Text.Json;

namespace SolarWatch.Service
{
    public class Geocoder : IGeocoder
    {
        private readonly ILogger<Geocoder> _logger;
        private readonly string _weatherApiKey;

        public Geocoder(ILogger<Geocoder> logger, IConfiguration config)
        {
            _logger = logger;
            _weatherApiKey = config["WeatherAPIKey"];
        }

        public async Task<Geocode> GetGeoCodesAsync(string city)
        {
            try
            {
                var geocodingUrl = $"http://api.openweathermap.org/geo/1.0/direct?q={city}&appid={_weatherApiKey}";
                using var client = new HttpClient();
                var response = await client.GetAsync(geocodingUrl);
                var cityData = await response.Content.ReadAsStringAsync();

                JsonDocument json = JsonDocument.Parse(cityData);
                double latitude = GetGeoDataFromJson(json, "lat");
                double longitude = GetGeoDataFromJson(json, "lon");
                string country = GetDataFromJson(json, "country");
                string? state = GetDataFromJson(json, "state") ?? "No state";

                return new Geocode(latitude, longitude, country, state);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, "An error occured while processing the geocode.");
                return new Geocode(double.NaN, double.NaN,"", "");
            }
        }

        private double GetGeoDataFromJson(JsonDocument json, string property)
        {
            double propToReturn = json.RootElement.EnumerateArray().FirstOrDefault().GetProperty(property).GetDouble();
            return propToReturn;
        }

        private string? GetDataFromJson(JsonDocument json, string property)
        {
            var firstArrayItem = json.RootElement.EnumerateArray().FirstOrDefault();

            if (firstArrayItem.TryGetProperty(property, out var propertyValue) && propertyValue.ValueKind != JsonValueKind.Null)
            {
                return propertyValue.GetString();
            }

            return null;
        }
    }
}
