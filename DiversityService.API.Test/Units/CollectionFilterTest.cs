namespace DiversityService.API.Test.Units
{
    using DiversityService.API.Model;
    using DiversityService.API.Services;
    using DiversityService.API.WebHost.Handler;
    using Moq;
    using Ninject;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Security.Claims;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Web.Http.Controllers;
    using System.Web.Http.Routing;
    using Xunit;

    public class CollectionFilterTest
    {
        private TestKernel Kernel = new TestKernel();

        private HttpRequestMessage Request = new HttpRequestMessage();
        private HttpRequestContext RequestContext = new HttpRequestContext();
        private HttpControllerContext ControllerContext;
        private HttpActionContext ActionContext;

        private Mock<IConfigurationService> Configuration;
        private Mock<IContextFactory> ContextFactory;

        private CollectionFilter Filter;

        public CollectionFilterTest()
        {
            InitializeActionContext();

            Configuration = Kernel.GetMock<IConfigurationService>();
            ContextFactory = Kernel.GetMock<IContextFactory>();

            Filter = Kernel.Get<CollectionFilter>();
        }

        [Fact]
        public async Task Rejects_Null_RouteData()
        {
            // Arrange

            // Act
            await InvokeFilter();

            // Assert
            Assert.False(ActionContext.Response.IsSuccessStatusCode);
        }

        [Fact]
        public async Task Rejects_Empty_RouteData()
        {
            // Arrange

            var routeData = new HttpRouteData(new HttpRoute(), new HttpRouteValueDictionary());
            Request.SetRouteData(routeData);

            // Act
            await InvokeFilter();

            // Assert
            Assert.False(ActionContext.Response.IsSuccessStatusCode);
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
            await InvokeFilter();

            // Assert
            Assert.False(ActionContext.Response.IsSuccessStatusCode);
        }

        [Fact]
        public async Task Rejects_Unknown_Collection()
        {
            // Arrange
            SetRouteData(0, 0);

            // Act
            await InvokeFilter();

            // Assert
            Assert.False(ActionContext.Response.IsSuccessStatusCode);
        }

        [Fact]
        public async Task Rejects_No_BackendCredentials()
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
            await InvokeFilter();

            // Assert
            Assert.False(ActionContext.Response.IsSuccessStatusCode);
        }

        [Fact]
        public async Task Rejects_Invalid_BackendCredentials()
        {
            // Arrange
            SetRouteData(0, 0);
            var servers = new[]{ new InternalCollectionServer() {
                Id = 0,
                Name = "Test"
            } };
            Configuration.Setup(x => x.GetCollectionServers())
                .Returns(servers);
            SetBackendCredentials("invalid", "user");
            ContextFactory.Setup(x => x.CreateContextAsync(servers[0], "invalid", "user"))
                .Returns(Task.FromResult<IContext>(null));

            // Act
            await InvokeFilter();

            // Assert
            Assert.False(ActionContext.Response.IsSuccessStatusCode);
        }

        private void InitializeActionContext()
        {
            RequestContext.Principal = new ClaimsPrincipal(new ClaimsIdentity());
            ControllerContext = new HttpControllerContext() { Request = Request, RequestContext = RequestContext };
            ActionContext = new HttpActionContext { ControllerContext = ControllerContext };
        }

        private void SetRouteData(int collectionId, int projectId)
        {
            var routeValues = new HttpRouteValueDictionary();
            routeValues[CollectionAPI.COLLECTION] = collectionId.ToString();
            routeValues[CollectionAPI.PROJECT] = projectId.ToString();
            var routeData = new HttpRouteData(new HttpRoute(), routeValues);
            Request.SetRouteData(routeData);
        }

        private void SetBackendCredentials(string user, string password)
        {
            var identity = RequestContext.Principal.Identity as ClaimsIdentity;
            identity.AddClaim(new BackendCredentialsClaim(user, password));
        }

        private Task InvokeFilter()
        {
            return Filter.OnActionExecutingAsync(ActionContext, CancellationToken.None);
        }
    }
}