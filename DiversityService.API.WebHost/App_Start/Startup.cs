namespace DiversityService.API.WebHost
{
    using Microsoft.Owin;
    using Microsoft.Owin.Security;
    using Ninject;
    using Owin;

    public partial class Startup
    {
        public virtual void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
            ConfigureNinject(app);
            ConfigureWebApi(app);
        }
    }

    public partial class TestStartup : Startup
    {
        public TestStartup()
        {
        }

        public TestStartup(IKernel kernel)
        {
            InitializeKernelIfNecessary(kernel, testing: true);

            kernel.Bind<ISecureDataFormat<AuthenticationTicket>>()
                .ToMethod(_ => Startup.OAuthOptions.AccessTokenFormat)
                .InSingletonScope();
        }

        public override void Configuration(IAppBuilder app)
        {
            ConfigureTestAuth(app);
            ConfigureNinject(app);
            ConfigureWebApi(app, testing: true);
        }
    }
}