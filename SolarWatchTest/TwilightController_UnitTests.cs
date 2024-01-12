using System;
using System.Globalization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SolarWatch.Contracts;
using SolarWatch.Controllers;
using SolarWatch.Model;
using SolarWatch.Service;

[TestFixture]
public class TwilightControllerTests
{
    [Test]
    public async Task Get_ValidCityAndDate_ShouldReturnOkResult()
    {
        // Arrange 
        var loggerMock = new Mock<ILogger<TwilightController>>();
        var geocoderMock = new Mock<IGeocoder>();
        var twilightDataFetcherMock = new Mock<ITwilightDataFetcher>();
        var cityRepositoryMock = new Mock<ICityRepository>();

        var TwilightController = new TwilightController(loggerMock.Object, geocoderMock.Object, twilightDataFetcherMock.Object, cityRepositoryMock.Object);

        var cityName = "Budapest";
        var date = "2023-12-01";

        SolarWatchRequestDTO slrwtchDTO = new SolarWatchRequestDTO(cityName, date);

        var geocode = new Geocode(47.49801, 19.03991, "HU");

        geocoderMock.Setup(g => g.GetGeoCodesAsync(cityName)).ReturnsAsync(geocode);

        var urlWithDate = $"https://api.sunrise-sunset.org/json?lat={geocode.Latitude}&lng={geocode.Longitude}&date={date}";
        var sunrise = "6:30:00 AM";
        var sunset = "6:30:00 PM";
        var solarNoon = "12:30:00 PM";
        var twilightData = new Twilight(sunrise, sunset, solarNoon);
        twilightDataFetcherMock.Setup(t => t.GetTwilightDataAsync(urlWithDate)).ReturnsAsync(twilightData);

        // Act

        var result = await TwilightController.Get(slrwtchDTO) as OkObjectResult;

        // Assert
        Assert.NotNull(result);
        Assert.AreEqual(200, result.StatusCode);
        Assert.NotNull(result.Value);
    }

    [Test]
    public async Task Get_InvalidCity_ShouldReturnBadRequestResult()
    {
        // Arrange
        var loggerMock = new Mock<ILogger<TwilightController>>();
        var geocoderMock = new Mock<IGeocoder>();
        var twilightDataFetcherMock = new Mock<ITwilightDataFetcher>();
        var cityRepositoryMock = new Mock<ICityRepository>();

        var TwilightController = new TwilightController(loggerMock.Object, geocoderMock.Object, twilightDataFetcherMock.Object, cityRepositoryMock.Object);

        var cityName = "NonExistentCity";
        var date = "2023-12-01";

        SolarWatchRequestDTO slrwtchDTO = new SolarWatchRequestDTO(cityName, date);

        geocoderMock.Setup(g => g.GetGeoCodesAsync(cityName)).ThrowsAsync(new Exception("City not found"));

        // Act
        var result = await TwilightController.Get(slrwtchDTO) as BadRequestObjectResult;

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(400, result.StatusCode);
        Assert.AreEqual("Something went wrong - City not found", result.Value);
    }

    [Test]
    public async Task Get_InvalidDateString_ShouldReturnBadRequestResult()
    {
        // Arrange
        var loggerMock = new Mock<ILogger<TwilightController>>();
        var geocoderMock = new Mock<IGeocoder>();
        var twilightDataFetcherMock = new Mock<ITwilightDataFetcher>();
        var cityRepositoryMock = new Mock<ICityRepository>();

        var TwilightController = new TwilightController(loggerMock.Object, geocoderMock.Object, twilightDataFetcherMock.Object, cityRepositoryMock.Object);

        var cityName = "ValidCity";
        var invalidDate = "2023-123-01"; // Invalid date format

        SolarWatchRequestDTO slrwtchDTO = new SolarWatchRequestDTO(cityName, invalidDate);

        // Act
        var result = await TwilightController.Get(slrwtchDTO) as BadRequestObjectResult;

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(400, result.StatusCode);
        Assert.AreEqual("Something went wrong - Invalid date format", result.Value);
    }
}

