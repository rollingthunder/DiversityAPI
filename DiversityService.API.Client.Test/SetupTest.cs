namespace DiversityService.API.Client.Test
{
    using DiversityService.API.Controllers;
    using DiversityService.API.Model;
    using DiversityService.API.Test;
    using DiversityService.API.WebHost;
    using DiversityService.API.WebHost.Models;
    using Ninject;
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Xunit;

    public class SetupTest : IDisposable
    {
        private TestAPI API;
        private DiversityAPI Client;

        public SetupTest()
        {
            API = new TestAPI();

            Client = new DiversityAPI(TestStartup.AuthorizationToken, API.Browser);
        }

        public void Dispose()
        {
            API.Dispose();
        }

        [Fact]
        public async Task CanSetCredentials()
        {
            // Arrange
            await API.Browser.InitializeAsync();
            var user = "user";
            var pass = "pass";
            var acc = API.Kernel.Get<AccountController>();
            var store = new TestUserStore();
            var appUser = new ApplicationUser()
            {
                UserName = TestStartup.TestUserName,
                Id = TestStartup.TestUserName
            };
            store.Users.Add(appUser);
            acc.UserManager = new ApplicationUserManager(store);

            // Act
            await Client.SetBackendCredentialsAsync(user, pass);

            // Assert
            Assert.True(appUser.Claims.Any(c => c.ClaimType == BackendCredentialsClaim.TYPE));
        }
    }
}