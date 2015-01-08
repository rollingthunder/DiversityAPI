namespace DiversityService.API.Test
{
    using DiversityService.API.Controllers;
    using DiversityService.API.Filters;
    using DiversityService.API.Model;
    using DiversityService.API.Services;
    using Moq;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Xunit;

    public class ProjectControllerTest : ControllerTestBase<ProjectController>
    {
        private Mock<IProjectStore> Projects;
        private Mock<IContext> CollectionContext;

        public ProjectControllerTest()
        {
            Projects = Kernel.GetMock<IProjectStore>();
            InitController();
            CollectionContext = CreateCollectionContext();
        }

        [Fact]
        public void Requires_a_Collection()
        {
            // Arrange

            // Act
            var hasCollectionFilter = (typeof(ProjectController).GetCustomAttributes(typeof(CollectionAPIAttribute), true)).Any();

            // Assert
            Assert.True(hasCollectionFilter);
        }

        [Fact]
        public async Task Returns_Projects_For_Collection()
        {
            // Arrange
            var dbProjects = new[] {
                new Collection.Project(){ProjectID = 0, DisplayText = "P1"},
                new Collection.Project(){ProjectID = 1, DisplayText = "P2"},
                new Collection.Project(){ProjectID = 2, DisplayText = "P3"},
            };

            Projects
                .SetupWithFakeData<IProjectStore, Collection.Project, int>(dbProjects.AsQueryable());

            // Act
            var publicProjects = await Controller.Get();

            // Assert
            Assert.Equal(dbProjects.Length, publicProjects.Count());
            Assert.True(
                // Each project should surface
                // with the same ID
               dbProjects
               .All(p => publicProjects.Any(x => x.Id == p.ProjectID))
           );
        }

        [Fact]
        public async Task Returns_A_Project_By_Id()
        {
            // Arrange
            var dbProjects = new[] {
                new Collection.Project(){ProjectID = 0, DisplayText = "P1"},
                new Collection.Project(){ProjectID = 1, DisplayText = "P2"},
                new Collection.Project(){ProjectID = 2, DisplayText = "P3"},
            };

            Projects
                .SetupWithFakeData<IProjectStore, Collection.Project, int>(dbProjects.AsQueryable());

            // Act
            var project = await Controller.GetById(0);

            // Assert
            Assert.Equal(0, project.Id);
        }
    }
}