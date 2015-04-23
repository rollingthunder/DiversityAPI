namespace DiversityService.API.WebHost
{
    using Ninject;
    using Ninject.Web.Common.OwinHost;
    using Owin;
    using System.Reflection;
    using System.Web.Http;

    public partial class Startup
    {
        protected IKernel Kernel;

        public void ConfigureNinject(IAppBuilder app)
        {
            Kernel = Kernel ?? new StandardKernel();

            Kernel.Load(Assembly.GetExecutingAssembly());

            app.UseNinjectMiddleware(() => Kernel);
        }
    }
}