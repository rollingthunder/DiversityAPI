namespace DiversityService.API.WebHost
{
    using Ninject;
    using Ninject.Web.Common.OwinHost;
    using Owin;
    using System.Reflection;
    using System.Web.Http;

    public partial class Startup
    {
        private IKernel Kernel;

        private IKernel CreateKernel()
        {
            var kernel = new StandardKernel();

            kernel.Load(Assembly.GetExecutingAssembly());

            return kernel;
        }

        public void ConfigureNinject(IAppBuilder app)
        {
            Kernel = CreateKernel();

            app.UseNinjectMiddleware(() => Kernel);
        }
    }
}