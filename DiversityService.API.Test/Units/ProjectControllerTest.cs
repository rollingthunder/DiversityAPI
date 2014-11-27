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
        private Mock<IMappingService> Mapper;
        private Mock<IContext> CollectionContext;

        public ProjectControllerTest()
        {
            Projects = Kernel.GetMock<IProjectStore>();
            Mapper = Kernel.GetMock<IMappingService>();
            InitController();
            CollectionContext = CreateCollectionContext();
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
            var dbProjects = new[] {
                new Collection.Project(){ProjectID = 0, DisplayText = "P1"},
                new Collection.Project(){ProjectID = 1, DisplayText = "P2"},
                new Collection.Project(){ProjectID = 2, DisplayText = "P3"},
            };

            var projects = new[] {
                new Project() {Id = 0, Name = "P1" },
                new Project() {Id = 1, Name = "P2" },
                new Project() {Id = 2, Name = "P3" },
            };

            Projects
                .SetupWithFakeData<IProjectStore, Collection.Project, int>(dbProjects.AsQueryable());

            IEnumerable<Collection.Project> mapped = Enumerable.Empty<Collection.Project>();
            Mapper
                .Setup(x => x.Map<IEnumerable<Project>>(It.IsAny<IEnumerable<Collection.Project>>()))
                .Callback((object x) => mapped = x as IEnumerable<Collection.Project>)
                .Returns(projects);

            // Act
            var publicProjects = await Controller.Get();

            // Assert
            Assert.True(TestHelper.AreSetEqual(mapped, dbProjects));
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