namespace DiversityService.API.Test.Units
{
    using DiversityService.API.Filters;
    using System.Net;
    using System.Net.Http;
    using System.Threading.Tasks;
    using System.Web.Http;
    using System.Web.Http.Controllers;
    using System.Web.Http.Hosting;
    using System.Web.Http.Routing;
    using Xunit;

    public class ValidateModelFilterTest
    {
        private ValidateModelAttribute Filter;

        public ValidateModelFilterTest()
        {
            Filter = new ValidateModelAttribute();
        }

        private HttpActionContext InitializeActionContext(HttpRequestMessage request)
        {
            var configuration = new HttpConfiguration();
            var route = configuration.Routes.MapHttpRoute(name: "DefaultApi", routeTemplate: "api/{controller}/{id}", defaults: new { id = RouteParameter.Optional });
            var routeData = new HttpRouteData(route, new HttpRouteValueDictionary { { "controller", "Issues" } });
            request.Properties[HttpPropertyKeys.HttpRouteDataKey] = routeData;
            var controllerContext = new HttpControllerContext(configuration, routeData, request);
            var actionContext = new HttpActionContext { ControllerContext = controllerContext };
            return actionContext;
        }

        [Fact]
        public async Task Returns_BadRequest_For_Invalid_ModelState()
        {
            // Arrange
            var request = new HttpRequestMessage();
            var context = InitializeActionContext(request);
            context.ModelState.AddModelError("testerror", "Err");

            // Act
            Filter.OnActionExecuting(context);

            // Assert
            Assert.NotNull(context.Response);
            Assert.Equal(HttpStatusCode.BadRequest, context.Response.StatusCode);
            Assert.Contains("Err", await context.Response.Content.ReadAsStringAsync());
        }

        [Fact]
        public void Changes_Nothing_With_Valid_ModelState()
        {
            // Arrange
            var request = new HttpRequestMessage();
            var context = InitializeActionContext(request);

            // Act
            Filter.OnActionExecuting(context);

            // Assert
            Assert.Null(context.Response);
        }
    }
}