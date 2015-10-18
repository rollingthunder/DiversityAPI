namespace DiversityService.API.Client.Test
{
    using System.Threading.Tasks;
    using Xunit;

    public class BrowserTest
    {
        private TestAPI API;
        private APIBrowser Browser;

        public BrowserTest()
        {
            API = new TestAPI();
            Browser = new APIBrowser(API.HttpClient);
        }

        [Fact]
        public async Task CanInitialize()
        {
            // Arrange
            // Act
            await Browser.InitializeAsync();
            // Assert
            Assert.NotNull(Browser.Home);
        }

        [Fact]
        public async Task CanGetAccountPrefix()
        {
            // Arrange
            await Browser.InitializeAsync();

            // Act
            var accUri = await Browser.GetAccountUriAsync();

            // Assert
            Assert.NotNull(accUri);
        }
    }
}