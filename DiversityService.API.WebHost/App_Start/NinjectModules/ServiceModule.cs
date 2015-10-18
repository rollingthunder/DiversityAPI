namespace DiversityService.API.WebHost
{
    using DiversityService.API.Services;
    using Microsoft.Owin.Security;
    using Ninject.Modules;
    using Ninject.Web.Common;

    public class ServiceModule : NinjectModule
    {
        public override void Load()
        {
            Bind<ISecureDataFormat<AuthenticationTicket>>()
                .ToMethod(_ => Startup.OAuthOptions.AccessTokenFormat)
                .InSingletonScope();

            Bind<IContextFactory>()
                .To<ContextFactory>()
                .InSingletonScope();

            Bind<IConfigurationService>()
                .To<ConfigurationService>()
                .InSingletonScope();

            // Data Stores 
            Bind<IProjectStore>()
                .To<ProjectStore>()
                .InRequestScope();

            Bind(typeof(IStore<,>))
                .To(typeof(Store<,>))
                .InRequestScope();
        }
    }
}