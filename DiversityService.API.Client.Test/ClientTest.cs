namespace DiversityService.API.Client.Test
{
    using Xunit;

    public class ClientTest
    {
        private const string USER = "Test";
        private const string PASSWORD = "Pass";
        private const string TOKEN = "{arstarstarsnthiarensh}";

        private DiversityAPI Client = new DiversityAPI(TOKEN);

        [Fact]
        public void CanSetupClient()
        {
            // Arrange
            // Act
            // Assert
            Assert.NotNull(Client);
        }
    }
}