namespace SolarWatch.Model
{
    public class Geocode
    {
        public double Latitude { get; }
        public double Longitude { get; }
        public string Country { get; }
        public string State { get; }
        public Geocode(double latitude, double longitude, string country, string state = "No State")
        {
            Latitude = latitude;
            Longitude = longitude;
            Country = country;
            State = state;
        }
    }
}
