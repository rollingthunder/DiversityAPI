namespace DiversityService.API.Test
{
    using DiversityService.API.Controllers;
    using DiversityService.API.Filters;
    using DiversityService.API.Services;
    using Moq;
    using System.Linq;
    using Xunit;

    public class ProjectControllerTest : ControllerTestBase<ProjectController>
    {
        private Mock<IProjectStore> Projects;

        public ProjectControllerTest()
        {
            Projects = Kernel.GetMock<IProjectStore>();
        }

        [Fact]
        public void Requires_a_Collection()
        {
            // Arrange

            // Act
            var hasCollectionFilter = (typeof(ProjectController).GetCustomAttributes(typeof(RequireCollectionAttribute), true)).Any();

            // Assert
            Assert.True(hasCollectionFilter);
        }

        [Fact]
        public void Returns_Projects_For_Collection()
        {
            // Arrange

            InitController();
            // Act
            // Assert
            Assert.True(false);
        }
    }
}