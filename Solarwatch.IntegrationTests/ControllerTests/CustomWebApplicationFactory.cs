using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using SolarWatch.Controllers;
using SolarWatch.Service;
using SolarWatch.Service.Authentication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Solarwatch.IntegrationTests.ControllerTests
{
    public class CustomWebApplicationFactory : WebApplicationFactory<Program>
    {
        public Mock<ILogger<TwilightController>> TwilightControllerMock {get;}
        public Mock<IGeocoder> GeocoderMock { get; }
        public Mock<ITwilightDataFetcher> DataFetcherMock { get; }
        public Mock<ICityRepository> CityRepositoryMock { get; }
        public Mock<IAuthenticationService> AuthenticationServiceMock { get; }

        public CustomWebApplicationFactory()
        {
            TwilightControllerMock = new Mock<ILogger<TwilightController>>();
            GeocoderMock = new Mock<IGeocoder>();
            DataFetcherMock = new Mock<ITwilightDataFetcher>();
            CityRepositoryMock = new Mock<ICityRepository>();
            AuthenticationServiceMock = new Mock<IAuthenticationService>();
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            base.ConfigureWebHost(builder);

            builder.ConfigureTestServices(services =>
            {
                services.AddSingleton(TwilightControllerMock.Object);
                services.AddSingleton(GeocoderMock.Object);
                services.AddSingleton(DataFetcherMock.Object);
                services.AddSingleton(CityRepositoryMock.Object);
                services.AddSingleton(AuthenticationServiceMock);
            });
        }
    }
}
