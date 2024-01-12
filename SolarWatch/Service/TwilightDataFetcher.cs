using SolarWatch.Model;
using System.Globalization;
using System.Net;
using System.Text.Json;

namespace SolarWatch.Service
{
    public class TwilightDataFetcher : ITwilightDataFetcher
    {
        private readonly ILogger<TwilightDataFetcher> _logger;

        public TwilightDataFetcher(ILogger<TwilightDataFetcher> logger)
        {
            _logger = logger;
        }

        public async Task<Twilight> GetTwilightDataAsync(string url)
        {
            try
            {
                using var client = new HttpClient();
                var response = await client.GetAsync(url);
                var twilightData = await response.Content.ReadAsStringAsync();
                JsonDocument json = JsonDocument.Parse(twilightData);
                var results = json.RootElement.GetProperty("results");
                string sunrise = results.GetProperty("sunrise").GetString();
                string sunset = results.GetProperty("sunset").GetString();
                string solarNoon = results.GetProperty("solar_noon").GetString();

                TimeOnly sunriseUTCOffset = ConvertTimeStringToExactTimeOnly(sunrise);
                TimeOnly sunsetUTCOffset = ConvertTimeStringToExactTimeOnly(sunset);
                TimeOnly solarNoonUTCOffset = ConvertTimeStringToExactTimeOnly(solarNoon);


                Twilight twilight = new Twilight(sunriseUTCOffset.ToString("h:mm:ss tt"), sunsetUTCOffset.ToString("h:mm:ss tt"), solarNoonUTCOffset.ToString("h:mm:ss tt"));
                return twilight;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, "An error occurred while processing twilight data.");
                throw new Exception(ex.Message);
            }
        }

        private TimeOnly ConvertTimeStringToExactTimeOnly(string timeString)
        {
            TimeOnly UTCTime = TimeOnly.ParseExact(timeString, "h:mm:ss tt", CultureInfo.InvariantCulture);
            TimeOnly UTCCorrectedTime = SetLocalUTCTime(UTCTime);
            return UTCCorrectedTime;
        }

        private TimeOnly SetLocalUTCTime(TimeOnly time)
        {
            TimeZone localZone = TimeZone.CurrentTimeZone;
            TimeSpan currentOffset = localZone.GetUtcOffset(DateTime.Now);
            return time.Add(currentOffset);
        }
    }
}
