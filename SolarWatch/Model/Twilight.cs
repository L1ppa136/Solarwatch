using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;
using System.Text.Json.Serialization;

namespace SolarWatch.Model
{
    public class Twilight
    {
        [Key]
        public int TwilightId { get; set; }
        [JsonIgnore]
        public City? City { get; private set; }
        public string? Date { get; private set; }

        [ForeignKey("City")]
        public int CityId { get; private set; }
        public string Sunrise { get; private set; }
        public string Sunset { get; private set; }
        public string SolarNoon { get; private set; }
        [NotMapped]
        public string? WeatherDescription { get; private set; }

        public Twilight(string sunrise, string sunset, string solarNoon)
        {
            Sunrise = sunrise;
            Sunset = sunset;
            SolarNoon = solarNoon;
        }

        public void SetCity(City city)
        {
            City = city;
            CityId = city.CityId;
        }

        public void SetDate(string date)
        {
            if (date == "today")
            {
                Date = DateTime.Now.ToString("yyyy-MM-dd");
            }
            else
            {
                Date = date;
            }
        }
        
        public void ChangeTwilightData(string sunrise, string sunset, string solarNoon)
        {
            Sunrise = sunrise;
            Sunset = sunset;
            SolarNoon = solarNoon;
        }

        public void AddWeatherDescription(string description)
        {
            WeatherDescription = description;
        }
    }
}
