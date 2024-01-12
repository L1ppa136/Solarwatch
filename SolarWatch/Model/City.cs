using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace SolarWatch.Model
{
    public class City
    {
        [Key]
        public int CityId { get; set; }
        public string Name { get; init; }
        public double Latitude {  get; init; }

        public double Longitude { get; init; }

        public string? State { get; init; }

        public string Country { get; init; }
        //[JsonIgnore]
        public List<Twilight> Twilights { get; private set; }

        public City(string name, double latitude, double longitude, string? state, string country)
        {
            Name = name;
            Latitude = latitude;
            Longitude = longitude;
            State = state;
            Country = country;
            Twilights = new List<Twilight>();
        }

        public void AddTwilightData(Twilight twilight)               
        {
            Twilight twilightToAdd = GetTwilightByDate(twilight.Date);
            if (twilightToAdd != null)
            {
                twilightToAdd.ChangeTwilightData(twilight.Sunrise, twilight.Sunset, twilight.SolarNoon);
            }
            else
            {
                twilight.SetCity(this);
                Twilights.Add(twilight);
            }
        }

        public Twilight? GetTwilightByDate(string date)
        {
            Twilight? twilight;
            if (date == "today")
            {
                string formattedDate = DateTime.Now.ToString("yyyy-MM-dd");
                twilight = Twilights.FirstOrDefault(t => t.Date == formattedDate);
            }
            else
            {
                twilight = Twilights.FirstOrDefault(t => t.Date == date);
            }
            
            return twilight;
        }
    }
}
