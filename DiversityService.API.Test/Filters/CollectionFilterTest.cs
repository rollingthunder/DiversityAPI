namespace DiversityService.API.Test.Units
{
    using DiversityService.API.Filters;
    using DiversityService.API.Model;
    using DiversityService.API.Services;
    using Moq;
    using Ninject;
    using System.Net;
    using System.Net.Http;
    using System.Security.Claims;
    using System.Threading.Tasks;
    using System.Web.Http.Routing;
    using Xunit;

    public class CollectionFilterTest : FilterTestBase<CollectionContextFilter>
    {
        private Mock<IConfigurationService> Configuration;
        private Mock<IContextFactory> ContextFactory;
        private Mock<IProjectStore> ProjectStore;

        private const int COLLECTION_ID = 0;
        private const int PROJECT_ID = 0;
        private const string USER = "user";
        private const string PASS = "pass";
        private IContext Context;

        public CollectionFilterTest()
        {
            InitializeActionContext();

            Configuration = Kernel.GetMock<IConfigurationService>();
            ContextFactory = Kernel.GetMock<IContextFactory>();
            ProjectStore = Kernel.GetMock<IProjectStore>();
            ProjectStore
                .Setup(x => x.GetByIDAsync(It.IsAny<int>()))
                .ReturnsAsync(null);

            Filter = Kernel.Get<CollectionContextFilter>();
        }

        [Fact]
        public async Task Rejects_Null_RouteData()
        {
            // Arrange

            // Act
            await ActionExecuting();

            // Assert
            Assert.False(ActionCalled());
            Assert.Equal(HttpStatusCode.InternalServerError, ActionContext.Response.StatusCode);
        }

        [Fact]
        public async Task Handles_Empty_RouteData()
        {
            // Arrange

            var routeData = new HttpRouteData(new HttpRoute(), new HttpRouteValueDictionary());
            Request.SetRouteData(routeData);

            // Act
            await ActionExecuting();

            // Assert
            Assert.True(ActionCalled());
            Assert.False(ContextSet());
        }

        [Fact]
        public async Task Rejects_Bad_RouteData()
        {
            // Arrange

            var routeValues = new HttpRouteValueDictionary();
            routeValues[CollectionAPI.COLLECTION] = "not an int";
            routeValues[CollectionAPI.PROJECT] = "6";
            var routeData = new HttpRouteData(new HttpRoute(), routeValues);
            Request.SetRouteData(routeData);

            // Act
            await ActionExecuting();

            // Assert
            Assert.False(ActionCalled());
            Assert.Equal(HttpStatusCode.BadRequest, ActionContext.Response.StatusCode);
        }

        [Fact]
        public async Task Rejects_Unknown_Collection()
        {
            // Arrange
            SetRouteData(COLLECTION_ID, PROJECT_ID);

            // Act
            await ActionExecuting();

            // Assert
            Assert.False(ActionCalled());
            Assert.Equal(HttpStatusCode.NotFound, ActionContext.Response.StatusCode);
        }

        [Fact]
        public async Task Rejects_Collection_But_No_BackendCredentials()
        {
            // Arrange
            SetRouteData(0, 0);
            var servers = new[]{ new InternalCollectionServer() {
                Id = 0,
                Name = "Test"
            } };
            Configuration.Setup(x => x.GetCollectionServers())
                .Returns(servers);

            // Act
            await ActionExecuting();

            // Assert
            Assert.False(ActionCalled());
            Assert.False(ActionContext.Response.IsSuccessStatusCode);
        }

        [Fact]
        public async Task Rejects_Collection_But_Invalid_BackendCredentials()
        {
            // Arrange
            SetValidCollectionAndProject();
            SetRouteData(COLLECTION_ID, PROJECT_ID);
            SetBackendCredentials("invalid", "user");

            // Act
            await ActionExecuting();

            // Assert
            Assert.False(ActionCalled());
            Assert.False(ActionContext.Response.IsSuccessStatusCode);
        }

        [Fact]
        public async Task Does_Not_Set_Project_Without_An_Id()
        {
            // Arrange
            SetValidCollectionAndProject();
            SetRouteData(COLLECTION_ID);
            SetBackendCredentials(USER, PASS);

            // Act
            await ActionExecuting();

            // Assert
            var ctx = Request.GetCollectionContext();
            Assert.Null(ctx.ProjectId);
        }

        [Fact]
        public async Task Sets_Project_With_An_Id()
        {
            // Arrange
            SetValidCollectionAndProject();
            SetRouteData(COLLECTION_ID, PROJECT_ID);
            SetBackendCredentials(USER, PASS);

            // Act
            await ActionExecuting();

            // Assert
            var ctx = Request.GetCollectionContext();
            Assert.NotNull(ctx.ProjectId);
        }

        [Fact]
        public async Task Rejects_Invalid_ProjectId()
        {
            // Arrange
            SetValidCollectionAndProject();
            SetRouteData(COLLECTION_ID, 10000);
            SetBackendCredentials(USER, PASS);

            // Act
            await ActionExecuting();

            // Assert
            Assert.False(ActionCalled());
            Assert.Equal(HttpStatusCode.NotFound, ActionContext.Response.StatusCode);
        }

        [Fact]
        public async Task Creates_Context()
        {
            // Arrange
            SetValidCollectionAndProject();
            SetRouteData(COLLECTION_ID, PROJECT_ID);
            SetBackendCredentials(USER, PASS);

            // Act
            await ActionExecuting();

            // Assert
            Assert.True(ContextSet());
            Assert.True(ActionCalled());
        }

        private void SetRouteData(int collectionId, int? projectId = null)
        {
            var routeValues = new HttpRouteValueDictionary();
            routeValues[CollectionAPI.COLLECTION] = collectionId.ToString();
            if (projectId.HasValue)
            {
                routeValues[CollectionAPI.PROJECT] = projectId.ToString();
            }
            var routeData = new HttpRouteData(new HttpRoute(), routeValues);
            Request.SetRouteData(routeData);
        }

        private void SetBackendCredentials(string user, string password)
        {
            var identity = RequestContext.Principal.Identity as ClaimsIdentity;
            identity.AddClaim(new BackendCredentialsClaim(user, password));
        }

        private void SetValidCollectionAndProject()
        {
            var servers = new[]{ new InternalCollectionServer() {
                Id = COLLECTION_ID,
                Name = "Test"
            } };
            Configuration
                .Setup(x => x.GetCollectionServers())
                .Returns(servers);

            var contextMock = Kernel.GetMock<IContext>();

            ContextFactory
                .Setup(x => x.CreateContextAsync(It.IsAny<InternalCollectionServer>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.FromResult<IContext>(null));

            ContextFactory
                .Setup(x => x.CreateContextAsync(servers[0], USER, PASS))
                .Returns(Task.FromResult<IContext>(contextMock.Object));

            contextMock.SetupProperty(x => x.ProjectId);

            contextMock
                .Setup(x => x.Projects)
                .Returns(ProjectStore.Object);

            ProjectStore
                .Setup(x => x.GetByIDAsync(PROJECT_ID))
                .ReturnsAsync(new Collection.Project() { DisplayText = "Test", ProjectID = PROJECT_ID });

            Context = contextMock.Object;
        }

        private bool ContextSet()
        {
            return Request.GetCollectionContext() != null;
        }
    }
}