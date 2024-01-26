using SolarWatch.Model;
using System.Text.Json;

namespace SolarWatch.Service
{
    public class WeatherDescriptionProvider : IWeatherDescriptionProvider
    {
        private readonly string _weatherApiKey;
        private readonly string _openAIApiKey;
        public WeatherDescriptionProvider(IConfiguration configuration) 
        {
            _weatherApiKey = configuration["WeatherAPIKey"];
            _openAIApiKey = configuration["OpenAI_APIKey"];
        }
        public async Task<string> ProvideWeatherDescription(Geocode geocode)
        {
            string openAIUrl = "https://api.openai.com/v1/chat/completions";
            string prompt = await GetCurrentWeatherJSON(geocode);
            string sysMsg = "Provide a conversational description of the current weather in the given location based on the provided JSON!\r\n\r\nKeep the following requirements:\r\n- Provide temperature in either Celsius or Fahrenheit, whichever is more appropriate based on the location.\r\n- Never display the temperature in Kelvin.\r\n- always include wind direction and wind speed in the description\r\n- If deviation between \"feels_like\" and \"temp\" properties are not more than 2 °C, then keep \"feels_like\" property out of the conversation, otherwise you should mention it.\r\n- Keep the description short, but informative.\r\n- Give some advice on clothing in terms of the weather conditions.\r\n- Mention some popular sights and/or historical events randomly related to the location.";
            var data = new
            {
                model = "gpt-4",
                messages = new[]
                {
                    new { role = "system", content = sysMsg },
                    new { role = "user", content = prompt }
                }
            };

            using (var client = new HttpClient())
            {
                var json = JsonSerializer.Serialize(data);
                var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {_openAIApiKey}");

                var response = await client.PostAsync(openAIUrl, content);

                if (response.IsSuccessStatusCode)
                {
                    var responseData = await response.Content.ReadAsStringAsync();
                    var jsonFormat = JsonDocument.Parse(responseData);
                    var choicesElement = jsonFormat.RootElement.GetProperty("choices");
                    var firstChoiceElement = choicesElement.EnumerateArray().First();
                    var messageElement = firstChoiceElement.GetProperty("message");
                    var contentElement = messageElement.GetProperty("content");

                    string contentValue = contentElement.GetString();
                    return contentValue;
                }
                else
                {
                    throw new Exception($"HTTP Error: {response.StatusCode}");
                }
            }
        }

        private async Task<string> GetCurrentWeatherJSON(Geocode geocode)
        {
            string requestURL = $"https://api.openweathermap.org/data/2.5/weather?lat={geocode.Latitude}&lon={geocode.Longitude}&appid={_weatherApiKey}";

            using var client = new HttpClient();
            var response = await client.GetAsync(requestURL);
            var weatherData = await response.Content.ReadAsStringAsync();

            return weatherData;
        }
    }
}
