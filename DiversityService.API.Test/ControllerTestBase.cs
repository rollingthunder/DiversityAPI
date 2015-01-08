namespace DiversityService.API.Test
{
    using DiversityService.API.Services;
    using Microsoft.Owin;
    using Moq;
    using Ninject;
    using System.Net.Http;
    using System.Net.Http.Formatting;
    using System.Web.Http;
    using WebApiContrib.IoC.Ninject;

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
                DependencyResolver = new NinjectResolver(Kernel)
            };

            Controller.Request = new HttpRequestMessage();

            CreateCollectionContext();
        }

        protected Mock<IContext> CreateCollectionContext()
        {
            var mock = Kernel.GetMock<IContext>();

            mock.SetupGet(x => x.Projects)
                .Returns(Kernel.GetMock<IProjectStore>().Object);

            mock.SetupGet(x => x.Series)
               .Returns(Kernel.GetMock<IStore<Collection.EventSeries, int>>().Object);

            mock.SetupGet(x => x.Events)
               .Returns(Kernel.GetMock<IStore<Collection.Event, int>>().Object);

            mock.SetupGet(x => x.Specimen)
               .Returns(Kernel.GetMock<IStore<Collection.Specimen, int>>().Object);

            Controller.Request.SetOwinContext(new OwinContext());
            Controller.Request.SetCollectionContext(mock.Object);

            return mock;
        }
    }
}