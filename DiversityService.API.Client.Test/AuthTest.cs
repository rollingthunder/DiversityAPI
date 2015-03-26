namespace DiversityService.API.Client.Test
{
    using DiversityService.API.WebHost;
    using Microsoft.Owin.Hosting;
    using Microsoft.Owin.Testing;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Text;
    using System.Threading.Tasks;
    using Xunit;

    public class AuthTest
    {
        [Fact]
        public async Task CanAuthenticate()
        {
            // Arrange
            using (var server = TestServer.Create<Startup>())
            {
                // Act
                var response = await server.HttpClient.GetAsync("/api/Account/ExternalLogins");

                // Assert
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            }
        }
    }
}