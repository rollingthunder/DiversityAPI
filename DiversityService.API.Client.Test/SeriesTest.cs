﻿using System.Net.Http;
using System.Threading.Tasks;
using DiversityService.API.WebHost;
using Refit;
using Xunit;

namespace DiversityService.API.Client.Test
{
    public class SeriesTest
    {
        [Fact]
        public async Task CanGetAll()
        {
            // Arrange
            var api = new TestAPI("/collection/0/series");

            // Act
            var authenticatingHandler = new AuthenticationHandler(api.Server.Handler, () => Task.FromResult(TestStartup.AuthorizationToken));
            var authenticatedClient = new HttpClient(authenticatingHandler) { BaseAddress = api.Server.BaseAddress };
            var client = RestService.For<ISeriesAPI>(authenticatedClient);
            var all = await client.GetAll();

            // Assert
            Assert.NotEmpty(all);
        }
    }
}