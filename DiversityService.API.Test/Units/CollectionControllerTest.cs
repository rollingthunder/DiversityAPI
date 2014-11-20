namespace DiversityService.API.Test
{
    using DiversityService.API.Controllers;
    using DiversityService.API.Services;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Xunit;
    using Ninject;
    using Moq;
    using DiversityService.API.Model;

    public class CollectionControllerTest : ControllerTestBase<CollectionController>
    {
        Mock<IConfigurationService> Configuration;

        public CollectionControllerTest()
        {
            Configuration = Kernel.GetMock<IConfigurationService>();
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

            InitController();

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
    }
}