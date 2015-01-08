using DiversityService.API.Filters;
using DiversityService.API.Handler;
using Microsoft.Owin.Security.OAuth;
using Owin;
using System.Web.Http;
using System.Web.Http.Filters;
using WebApiContrib.Formatting.Siren.Client;

namespace DiversityService.API.WebHost
{
    public partial class Startup
    {
        public void ConfigureWebApi(IAppBuilder app)
        {
        }
    }

    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            RegisterFilters(config);
            RegisterRoutes(config);
        }

        public static void RegisterRoutes(HttpConfiguration config)
        {
            // Web API routes
            config.MapHttpAttributeRoutes();
        }

        public static void RegisterFilters(HttpConfiguration config)
        {
            // Web API configuration and services
            // Configure Web API to use only bearer token authentication.
            config.SuppressDefaultHostAuthentication();

            // Filters
            config.Filters.Add(new HostAuthenticationFilter(OAuthDefaults.AuthenticationType));
            config.Filters.Add(new AuthorizeAttribute());
            config.Filters.Add(new ValidateModelAttribute());
            config.Filters.Add(config.DependencyResolver.GetService(typeof(CollectionContextFilter)) as IFilter);
            // config.Filters.Add(new SirenResultAttribute());

            config.MessageHandlers.Add(new RequireHttpsMessageHandler());

            config.MessageHandlers.Add(new CultureHandler());

            config.Formatters.Add(new SirenJsonMediaTypeFormatter());
        }
    }
}