namespace DiversityService.API.Client.Test
{
    using Xunit;

    public class ClientTest
    {
        private const string USER = "Test";
        private const string PASSWORD = "Pass";

        private DiversityAPI Client = new DiversityAPI(USER, PASSWORD);

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