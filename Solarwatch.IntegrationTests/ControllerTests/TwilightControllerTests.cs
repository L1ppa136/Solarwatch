using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using SolarWatch.Contracts;
using SolarWatch.Model;
using SolarWatch.Controllers;
using SolarWatch.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Http.Json;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Authentication;

namespace Solarwatch.IntegrationTests.ControllerTests
{
    public class TwilightControllerTests : IDisposable
    {
        private CustomWebApplicationFactory _factory;
        private HttpClient _client;

        public TwilightControllerTests()
        {
            _factory = new CustomWebApplicationFactory();
            _client = _factory.CreateClient(); // connecting to OUR webservice in a test environment
        }

        [Test]
        public async Task Get_ValidCityAndDate_ShouldReturnOkResult()
        {
            // Arrange 
            var testCity = new City("Budapest", 47.4979937, 19.4979937, "No State", "HU");
            var testTwilight = new Twilight("7:07:59 de.", "3:58:33 du.", "11:33:16 de.");

            // Set up the behavior of your mock repository
            _factory.CityRepositoryMock.Setup(r => r.GetCityByNameAsync(It.IsAny<string>(), It.IsAny<bool>()))
                                        .ReturnsAsync(testCity);

            
            var cityName = "Budapest";
            var date = "2023-12-01";
            testTwilight.SetDate(date);
            testCity.AddTwilightData(testTwilight);
            SolarWatchRequestDTO slrwtchDTO = new SolarWatchRequestDTO(cityName, date);

            // Login with admin and get the access token by deserializing the loginRespoinse
            var loginResponse = await _client.PostAsJsonAsync("/Auth/Login", new AuthenticationRequest("admin@admin.com", "Admin123"));
            var authenticationResult = JsonConvert.DeserializeObject<AuthenticationResult>(await loginResponse.Content.ReadAsStringAsync());
            var accessToken = authenticationResult.Token;

            // Add authorization token to the request headers
            _client.DefaultRequestHeaders.Add("Authorization", $"Bearer {accessToken}");

            // Act
            var twilightResponse = await _client.PostAsJsonAsync("/Twilight/Solarwatch", slrwtchDTO);

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, twilightResponse.StatusCode);

            // Deserialize the response content and assert its structure/values
            var responseData = JsonConvert.DeserializeObject<Twilight>(await twilightResponse.Content.ReadAsStringAsync());
            Assert.IsNotNull(responseData);
            Assert.AreEqual(testTwilight.Sunrise, responseData.Sunrise);
            Assert.AreEqual(testTwilight.Sunset, responseData.Sunset);
        }

        [Test]
        public async Task Get_Cities_ShouldReturnOkResult()
        {
            // Arrange 
            string[] testCityNames = new string[] { "Budapest", "Bukarest", "London" };

            // Set up the behavior of your mock repository
            _factory.CityRepositoryMock.Setup(r => r.GetCities()).ReturnsAsync(testCityNames);

            // Act
            var response = await _client.GetAsync("/Twilight/Cities");

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

            // Deserialize the response content and assert its structure/values
            var responseData = JsonConvert.DeserializeObject<string[]>(await response.Content.ReadAsStringAsync());
            Assert.AreEqual(responseData[0], testCityNames[0]);
            Assert.AreEqual(responseData[1], testCityNames[1]);
            Assert.AreEqual(responseData[2], testCityNames[2]);
            Assert.AreEqual(responseData.Length, testCityNames.Length);
        }

        public void Dispose()
        {
            _client.Dispose();
            _factory.Dispose();
        }
    }
}
