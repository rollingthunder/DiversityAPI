namespace DiversityService.API.Test
{
    using DiversityService.API.Filters;
    using Moq;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Web.Http;
    using System.Web.Http.Controllers;
    using System.Web.Http.Dependencies;
    using System.Web.Http.Filters;
    using WebApiContrib.Formatting.Siren.Client;
    using Xunit;

    public class SirenResultFilterTest
    {
        private Mock<IDependencyResolver> DependencyResolver;
        private HttpConfiguration Configuration;
        private HttpRequestMessage Request;
        private HttpResponseMessage Response;
        private HttpControllerContext ControllerContext;
        private HttpActionContext ActionContext;
        private HttpActionExecutedContext ExecutedContext;

        private SirenResultAttribute Filter;

        public SirenResultFilterTest()
        {
            CreateContext();

            Filter = new SirenResultAttribute();
        }

        [Fact]
        public async Task Converts_results_to_Siren()
        {
            // Arrange

            // Act

            // Assert
            Assert.True(false);
        }

        [Fact]
        public async Task Calls_registered_SirenProviders()
        {
            // Arrange
            var obj = new object();
            var sirenDocument = new SirenEntity();
            var providers = new List<ISirenProvider>();

            var sirenProvider = new Mock<ISirenProvider>();
            providers.Add(sirenProvider.Object);
            sirenProvider.Setup(x => x.CanTranslate(typeof(object)))
                .Returns(true);
            sirenProvider.Setup(x => x.Translate(It.IsAny<object>()))
                .Returns(sirenDocument);

            var noSirenProvider = new Mock<ISirenProvider>();
            providers.Add(noSirenProvider.Object);
            noSirenProvider.Setup(x => x.CanTranslate(typeof(object)))
                .Returns(false);

            DependencyResolver
                .Setup(x => x.GetServices(typeof(ISirenProvider)))
                .Returns(providers);

            Response.Content = new ObjectContent<object>(obj, Configuration.Formatters.First());

            // Act
            await Filter.OnActionExecutedAsync(ExecutedContext, CancellationToken.None);

            // Assert
            sirenProvider.Verify(x => x.CanTranslate(typeof(object)), Times.Once());
            sirenProvider.Verify(x => x.Translate(obj), Times.Once());
            noSirenProvider.Verify(x => x.CanTranslate(typeof(object)), Times.AtMostOnce());
            noSirenProvider.Verify(x => x.Translate(obj), Times.Never());
            var newContent = Assert.IsAssignableFrom<ObjectContent>(ExecutedContext.Response.Content);
            Assert.Equal(sirenDocument, newContent.Value);
        }

        private void CreateContext()
        {
            DependencyResolver = new Mock<IDependencyResolver>();
            DependencyResolver
                .Setup(x => x.GetService(It.IsAny<Type>()))
                .Returns(null);

            DependencyResolver
                .Setup(x => x.GetServices(It.IsAny<Type>()))
                .Returns(Enumerable.Empty<object>());

            Configuration = new HttpConfiguration()
            {
                DependencyResolver = DependencyResolver.Object
            };

            Request = new HttpRequestMessage();

            Response = Request.CreateResponse();

            ControllerContext = new HttpControllerContext()
            {
                Request = Request,
                Configuration = Configuration
            };

            ActionContext = new HttpActionContext()
            {
                ControllerContext = ControllerContext,
                Response = Response
            };

            ExecutedContext = new HttpActionExecutedContext()
            {
                ActionContext = ActionContext,
                Response = Response
            };
        }
    }
}