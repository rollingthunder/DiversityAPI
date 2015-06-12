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

        public void InitializeKernelIfNecessary(IKernel useKernel = null, bool testing = false)
        {
            if (Kernel == null)
            {
                Kernel = useKernel ?? new StandardKernel();

                if (!testing)
                {
                    Kernel.Load<ServiceModule>();
                }

                Kernel.Load<MapperModule>();
            }
        }

        public void ConfigureNinject(IAppBuilder app)
        {
            InitializeKernelIfNecessary();
            app.UseNinjectMiddleware(() => Kernel);
        }
    }
}