namespace DiversityService.API.WebHost
{
    using System.Web.Http;
    using DiversityService.API.Filters;
    using DiversityService.API.Handler;
    using Microsoft.Owin.Security.OAuth;
    using Ninject;
    using Ninject.Web.WebApi.OwinHost;
    using Owin;

    public partial class Startup
    {
        public static void RegisterCommonFilters(IKernel kernel, HttpConfiguration config)
        {
            config.Filters.Add(new ValidateModelAttribute());
            config.Filters.Add(kernel.Get<CollectionContextFilter>());
            config.MessageHandlers.Add(new CultureHandler());
        }

        public static void RegisterProductionFilters(HttpConfiguration config)
        {
            // Web API configuration and services Configure Web API to use only bearer token authentication. 
            config.SuppressDefaultHostAuthentication();

            // Filters 
            config.Filters.Add(new HostAuthenticationFilter(OAuthDefaults.AuthenticationType));
            config.Filters.Add(new AuthorizeAttribute());

            config.MessageHandlers.Add(new RequireHttpsMessageHandler());
        }

        public static void RegisterRoutes(HttpConfiguration config)
        {
            // Web API routes 
            config.MapHttpAttributeRoutes();
        }

        public void ConfigureWebApi(IAppBuilder app, bool testing = false)
        {
            var config = new HttpConfiguration();

            if (!testing)
            {
                RegisterProductionFilters(config);

                ConfigureSwagger(config);
            }

            RegisterCommonFilters(Kernel, config);
            RegisterRoutes(config);

            app.UseNinjectWebApi(config);
        }
    }
}