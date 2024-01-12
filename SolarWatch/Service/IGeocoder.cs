using Microsoft.AspNetCore.Mvc;
using SolarWatch.Model;

namespace SolarWatch.Service
{
    public interface IGeocoder
    {
        Task<Geocode> GetGeoCodesAsync(string city);
    }
}
