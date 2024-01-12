using System.ComponentModel.DataAnnotations;

namespace SolarWatch.Contracts
{
    public record CityUpdateRequest([Required] string CityName, [Required]string Date, [Required] string Sunrise, [Required] string Sunset, [Required] string SolarNoon);
}
