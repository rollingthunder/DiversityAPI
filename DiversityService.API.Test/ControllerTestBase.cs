namespace DiversityService.API.Test
{
    using Ninject;
    using System.Web.Http;

    public class ControllerTestBase<T> : TestBase where T : ApiController
    {
        protected T Controller;

        protected void InitController()
        {
            Controller = Kernel.Get<T>();
        }
    }
}