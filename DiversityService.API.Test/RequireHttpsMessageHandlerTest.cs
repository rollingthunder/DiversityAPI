﻿namespace DiversityService.API.Test
{
    using DiversityService.API.Handler;
    using System.Net;
    using System.Net.Http;
    using System.Threading.Tasks;
    using Xunit;

    public class RequireHttpsMessageHandlerTest
    {
        [Fact]
        public async Task Returns_Redirect_If_Request_Is_Not_Over_HTTPS()
        {
            // Arange
            var request = new HttpRequestMessage(
                HttpMethod.Get, "http://localhost:8080"
            );
            var requireHtttpsMessageHandler = new RequireHttpsMessageHandler();
            // Act
            var response = await requireHtttpsMessageHandler.InvokeAsync(request);
            // Assert
            Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);
        }

        [Fact]
        public async Task Returns_Delegated_StatusCode_When_Request_Is_Over_HTTPS()
        {
            // Arange
            var request = new HttpRequestMessage(HttpMethod.Get, "https://localhost:8080");
            var requireHtttpsMessageHandler = new RequireHttpsMessageHandler();
            // Act
            var response = await requireHtttpsMessageHandler.InvokeAsync(request);
            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }
    }
}