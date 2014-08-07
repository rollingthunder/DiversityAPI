using DiversityService.API.WebHost.Handler;
using Microsoft.Owin.Security.OAuth;
using System.Web.Http;

namespace DiversityService.API.WebHost
{
    public static class WebApiConfig
    {
        public static void RegisterRoutes(HttpConfiguration config)
        { 
            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional });
        }

        public static void RegisterFilters(HttpConfiguration config)
        {
            // Web API configuration and services
            // Configure Web API to use only bearer token authentication.
            config.SuppressDefaultHostAuthentication();
            config.Filters.Add(new HostAuthenticationFilter(OAuthDefaults.AuthenticationType));

            config.Filters.Add(new AuthorizeAttribute());

            config.MessageHandlers.Add(new RequireHttpsMessageHandler());

            config.MessageHandlers.Add(new CultureHandler());
        }
    }
}
