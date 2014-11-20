namespace DiversityService.API.Test
{
    using DiversityService.API.Controllers;
    using DiversityService.API.Filters;
    using DiversityService.API.Model;
    using DiversityService.API.Services;
    using Moq;
    using System.Linq;
    using System.Threading.Tasks;
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
        public async Task Returns_Projects_For_Collection()
        {
            // Arrange
            var projects = new[] {
                new Project() {Id = 0, Name = "P1" },
                new Project() {Id = 1, Name = "P2" },
                new Project() {Id = 2, Name = "P3" },
            };

            Projects
                .Setup(x => x.GetProjectsAsync())
                .Returns(Task.FromResult(projects.AsEnumerable()));

            InitController();

            // Act
            var publicProjects = await Controller.Get();

            // Assert
            Assert.Equal(projects.Length, publicProjects.Count());
            Assert.True(
                // Each server should surface
                // with the same ID
               projects
               .All(p => publicProjects.Any(x => x.Id == p.Id))
           );
        }
    }
}