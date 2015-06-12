namespace DiversityService.API.Test
{
    using DiversityService.API.Model;
    using DiversityService.API.Services;
    using Microsoft.Owin;
    using Moq;
    using Ninject;
    using Ninject.Web.WebApi;
    using System.Net.Http;
    using System.Net.Http.Formatting;
    using System.Web.Http;

    public class ControllerTestBase<T> : TestBase where T : ApiController
    {
        protected T Controller;

        protected void InitController()
        {
            Controller = Kernel.Get<T>();

            Kernel.Bind<IContentNegotiator>()
                .ToConstant(new DefaultContentNegotiator());

            Controller.Configuration = new HttpConfiguration()
            {
                DependencyResolver = new NinjectDependencyResolver(Kernel)
            };

            Controller.Request = new HttpRequestMessage();

            var mock = Mocks.SetupContext(Kernel);

            // Set Request Context

            var octx = new OwinContext();

            Controller.Request.SetOwinContext(octx);
            Controller.Request.SetCollectionContext(mock.Object);

            var agent = new AgentInfo()
            {
                Name = "Test, U.",
                Uri = "testuri..."
            };
            Controller.Request.SetAgentInfo(agent);
        }
    }
}