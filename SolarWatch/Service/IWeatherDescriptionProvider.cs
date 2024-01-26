using SolarWatch.Model;

namespace SolarWatch.Service
{
    public interface IWeatherDescriptionProvider
    {
        Task<string> ProvideWeatherDescription(Geocode geocode);
    }
}
