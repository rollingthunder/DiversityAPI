namespace DiversityService.API.Test
{
    using DiversityService.API.Services;
    using Microsoft.Owin;
    using Moq;
    using Ninject;
    using System.Net.Http;
    using System.Web.Http;
    using System.Web.Http.Controllers;

    public class ControllerTestBase<T> : TestBase where T : ApiController
    {
        protected T Controller;

        protected void InitController()
        {
            Controller = Kernel.Get<T>();

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

            Controller.Request.SetOwinContext(new OwinContext());
            Controller.Request.SetCollectionContext(mock.Object);

            return mock;
        }
    }
}