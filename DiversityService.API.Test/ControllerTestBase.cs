namespace DiversityService.API.Test
{
    using DiversityService.API.Model;
    using DiversityService.API.Model.Internal;
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

        protected readonly OwinContext OwinContext;
        protected readonly HttpRequestMessage Request;
        protected readonly CollectionServerLogin Login;
        protected readonly AgentInfo Agent;

        public ControllerTestBase()
        {
            Request = new HttpRequestMessage();

            OwinContext = new OwinContext();
            Request.SetOwinContext(OwinContext);

            Login = new CollectionServerLogin()
            {
                Name = "ControllerTestBase Login",
                Address = "localhost",
                Id = 1,
                Password = "some",
                Port = 1234,
                User = "test"
            };
            Request.SetBackendCredentials(Login);

            Agent = new AgentInfo()
            {
                Name = "Test, U.",
                Uri = "testuri..."
            };
            Request.SetAgentInfo(Agent);
        }

        protected void InitController()
        {
            Controller = Kernel.Get<T>();

            Kernel.Bind<IContentNegotiator>()
                .ToConstant(new DefaultContentNegotiator());

            Controller.Configuration = new HttpConfiguration()
            {
                DependencyResolver = new NinjectDependencyResolver(Kernel)
            };

            Controller.Request = Request;

            var mock = SetupMocks.Context(Kernel);

            Controller.Request.SetCollectionContext(mock.Object);
        }
    }
}