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

            CreateCollectionContext();
        }

        protected Mock<IContext> CreateCollectionContext()
        {
            var mock = Kernel.GetMock<IContext>();

            // Transaction

            var transaction = Kernel.GetMock<ITransaction>();

            mock.Setup(x => x.BeginTransaction())
                .Returns(transaction.Object);

            // Stores

            mock.SetupGet(x => x.Projects)
                .Returns(Kernel.GetMock<IProjectStore>().Object);

            mock.SetupGet(x => x.Series)
               .Returns(Kernel.GetMock<IStore<Collection.EventSeries, int>>().Object);

            mock.SetupGet(x => x.Events)
               .Returns(Kernel.GetMock<IStore<Collection.Event, int>>().Object);

            mock.SetupGet(x => x.Specimen)
               .Returns(Kernel.GetMock<IStore<Collection.Specimen, int>>().Object);

            mock.SetupGet(x => x.IdentificationUnits)
               .Returns(Kernel.GetMock<IStore<Collection.IdentificationUnit, Collection.IdentificationUnitKey>>().Object);

            mock.SetupGet(x => x.Identifications)
               .Returns(Kernel.GetMock<IStore<Collection.Identification, Collection.IdentificationKey>>().Object);

            mock.SetupGet(x => x.IdentificationGeoAnalyses)
               .Returns(Kernel.GetMock<IStore<Collection.IdentificationUnitGeoAnalysis, Collection.IdentificationGeoKey>>().Object);

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

            return mock;
        }
    }
}