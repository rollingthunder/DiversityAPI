namespace DiversityService.API.Client.Test
{
    using DiversityService.API.WebHost;
    using Microsoft.Owin.Hosting;
    using Microsoft.Owin.Testing;
    using Newtonsoft.Json.Linq;
    using OpenQA.Selenium;
    using OpenQA.Selenium.Firefox;
    using OpenQA.Selenium.Interactions;
    using OpenQA.Selenium.Support.UI;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Text;
    using System.Threading.Tasks;
    using Xunit;

    public class AuthTest
    {
        private string PerformMSLogin(string msUri)
        {
            using (var Selenium = new FirefoxDriver())
            {
                Selenium.Navigate().GoToUrl(msUri);

                var userDiv = Selenium.FindElement(By.Id("idDiv_PWD_UsernameTb"));
                var user = userDiv.FindElement(By.TagName("input"));
                var passwordDiv = Selenium.FindElement(By.Id("idDiv_PWD_PasswordTb"));
                var password = passwordDiv.FindElement(By.TagName("input"));
                var submit = Selenium.FindElement(By.Id("idSIButton9"));

                user.SendKeys("diversityapi@outlook.com");
                password.SendKeys("jh4ROvh5m6jfTVW8");
                submit.Click();

                try
                {
                    var yesBtn = Selenium.FindElement(By.Id("idBtn_Accept"));
                    yesBtn.Click();
                }
                catch (NoSuchElementException)
                {
                    // already accepted ?
                }

                return Selenium.Url;
            }
        }

        [Fact]
        public async Task CanAuthenticateManually()
        {
            // Arrange
            using (var server = TestServer.Create<Startup>())
            {
                // Act
                var baseAddress = "https://diversityapi.de:44301";

                server.BaseAddress = new Uri(baseAddress);
                var response = await server.HttpClient.GetAsync("/api/Account/ExternalLogins?returnUrl=%2F&generateState=true");

                var logins = await response.Content.ReadAsAsync<JArray>();

                var login = logins[0];

                var url = login["Url"].ToString();

                var cookieHandler = new CookieHandler() { InnerHandler = server.Handler };

                var serverClient = new HttpClient(cookieHandler) { BaseAddress = new Uri(baseAddress) };

                var redirect = await serverClient.GetAsync(url);

                var msUri = redirect.Headers.Location.AbsoluteUri;

                var returnUri = PerformMSLogin(msUri);

                var signinResponse = await serverClient.GetAsync(returnUri);

                var loginUri = signinResponse.Headers.Location.AbsoluteUri.Replace(baseAddress, "");

                var loginResponse = await serverClient.GetAsync(loginUri);

                // Assert
                Assert.Equal(HttpStatusCode.Found, loginResponse.StatusCode);
                Assert.NotNull(loginResponse.Headers.Location.Fragment);
            }
        }

        [Fact]
        public async Task CanAuthenticateWithTheClient()
        {
            // Arrange
            using (var server = TestServer.Create<Startup>())
            {
                var baseAddress = new Uri("https://diversityapi.de:44301");
                server.BaseAddress = baseAddress;

                var client = new APIAuthentication(baseAddress, server.Handler);

                // Act
                var msUri = await client.GetLoginUriAsync();

                var returnUri = PerformMSLogin(msUri);

                var token = await client.AuthenticateReturnURLAsync(returnUri);

                // Assert
                Assert.NotNull(token);
            }
        }

        [Fact]
        public async Task CanAuthenticateUsingTestAuthentication()
        {
            // Arrange
            var api = new TestAPI("/Account/UserInfo");

            // Act
            var authenticatingHandler = new AuthenticationHandler(api.Server.Handler, async () => TestStartup.AuthorizationToken);
            var authenticatedClient = new HttpClient(authenticatingHandler) { BaseAddress = api.Server.BaseAddress };
            authenticatedClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer");
            authenticatedClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("*/*", 1.0));
            var response = await authenticatedClient.GetAsync("");

            // Assert
            Assert.True(response.IsSuccessStatusCode, response.ReasonPhrase);
            Assert.NotEqual(HttpStatusCode.Unauthorized, response.StatusCode);
        }
    }
}