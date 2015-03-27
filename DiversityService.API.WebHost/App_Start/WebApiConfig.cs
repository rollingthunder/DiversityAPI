using DiversityService.API.Filters;
using DiversityService.API.Handler;
using Microsoft.Owin.Security.OAuth;
using Ninject;
using Ninject.Web.Common.OwinHost;
using Ninject.Web.WebApi.OwinHost;
using Owin;
using System.Reflection;
using System.Web.Http;
using System.Web.Http.Filters;

namespace DiversityService.API.WebHost
{
    public static class WebApiConfig
    {
        public static void Register(IKernel kernel, HttpConfiguration config)
        {
            RegisterFilters(kernel, config);
            RegisterRoutes(config);
        }

        public static void RegisterRoutes(HttpConfiguration config)
        {
            // Web API routes
            config.MapHttpAttributeRoutes();
        }

        public static void RegisterFilters(IKernel kernel, HttpConfiguration config)
        {
            // Web API configuration and services
            // Configure Web API to use only bearer token authentication.
            config.SuppressDefaultHostAuthentication();

            // Filters
            config.Filters.Add(new HostAuthenticationFilter(OAuthDefaults.AuthenticationType));
            config.Filters.Add(new AuthorizeAttribute());
            config.Filters.Add(new ValidateModelAttribute());
            config.Filters.Add(kernel.Get<CollectionContextFilter>());
            // config.Filters.Add(new SirenResultAttribute());

            config.MessageHandlers.Add(new RequireHttpsMessageHandler());

            config.MessageHandlers.Add(new CultureHandler());
        }
    }
}