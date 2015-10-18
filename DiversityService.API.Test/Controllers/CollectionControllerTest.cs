namespace DiversityService.API.Test
{
    using DiversityService.API.Controllers;
    using DiversityService.API.Model.Internal;
    using DiversityService.API.Services;
    using Moq;
    using System.Linq;
    using Xunit;

    public class CollectionControllerTest : ControllerTestBase<CollectionController>
    {
        private Mock<IConfigurationService> Configuration;

        public CollectionControllerTest()
        {
            Configuration = Kernel.GetMock<IConfigurationService>();
            InitController();
        }

        [Fact]
        public void Returns_All_Servers()
        {
            // Arrange
            var servers = new[] {
                new InternalCollectionServer() { Id = 0, Name = "Server0" },
                new InternalCollectionServer() { Id = 1, Name = "Server1" },
                new InternalCollectionServer() { Id = 2, Name = "Server2" }
            };

            Configuration
                .Setup(x => x.GetCollectionServers())
                .Returns(servers);

            // Act
            var publicServers = Controller.Get();

            // Assert
            Assert.Equal(servers.Length, publicServers.Count());
            Assert.True(
                // Each server should surface
                // with the same ID
                servers
                .All(srv => publicServers.Any(x => x.Id == srv.Id))
            );
        }

        [Fact]
        public void Returns_Servers_By_Id()
        {
            // Arrange
            var servers = new[] {
                new InternalCollectionServer() { Id = 0, Name = "Server0" },
                new InternalCollectionServer() { Id = 1, Name = "Server1" },
                new InternalCollectionServer() { Id = 2, Name = "Server2" }
            };

            Configuration
                .Setup(x => x.GetCollectionServers())
                .Returns(servers);

            // Act
            var server = Controller.GetById(0);

            // Assert
            Assert.Equal(0, server.Id);
        }
    }
}