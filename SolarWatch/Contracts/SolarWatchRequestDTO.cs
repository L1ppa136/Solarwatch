using System.ComponentModel.DataAnnotations;

namespace SolarWatch.Contracts
{
    public record SolarWatchRequestDTO([Required]string CityName, string? Date);
}
