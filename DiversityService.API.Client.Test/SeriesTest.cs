using System;
using System.Net.Http;
using System.Threading.Tasks;
using DiversityService.API.Model;
using DiversityService.API.WebHost;
using Refit;
using Xunit;

namespace DiversityService.API.Client.Test
{
    public class SeriesTest
    {
        readonly TestAPI API;
        readonly HttpClient Client;

        public SeriesTest()
        {
            API = new TestAPI("/collection/0/series");

            var authenticatingHandler = new AuthenticationHandler(API.Server.Handler, () => Task.FromResult(TestStartup.AuthorizationToken));
            Client = new HttpClient(authenticatingHandler) { BaseAddress = API.Server.BaseAddress };
        }

        [Fact]
        public async Task CanGetAll()
        {
            // Arrange
            var series = RestService.For<ISeriesAPI>(Client);

            // Act
            var all = await series.GetAll();

            // Assert
            Assert.NotEmpty(all);
        }

        [Fact]
        public async Task CanInsert()
        {
            // Arrange
            var series = RestService.For<ISeriesAPI>(Client);
            var newSeries = new EventSeriesUpload()
            {
                TransactionGuid = Guid.NewGuid(),
                Description = "TestSeries"
            };

            // Act
            var es = await series.Create(newSeries);

            // Assert
            Assert.Equal(es.Description, newSeries.Description);
        }
    }
}