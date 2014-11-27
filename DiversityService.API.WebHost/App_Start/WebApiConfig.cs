﻿using DiversityService.API.Filters;
using DiversityService.API.Handler;
using Microsoft.Owin.Security.OAuth;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Dispatcher;
using WebApiContrib.Formatting.Siren.Client;

namespace DiversityService.API.WebHost
{
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

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { controller = "ApiHome", id = RouteParameter.Optional });

            var collectionApiHandler = config.DependencyResolver.GetService(typeof(CollectionFilter)) as CollectionFilter;

            config.Routes.MapHttpRoute(
                name: "CollectionApi",
                routeTemplate: "api/collection/{" + CollectionAPI.COLLECTION + "}/project/{" + CollectionAPI.PROJECT + "}/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
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
            config.Filters.Add(new EnablePagingAttribute());
            config.Filters.Add(new SirenResultAttribute());

            config.MessageHandlers.Add(new RequireHttpsMessageHandler());

            config.MessageHandlers.Add(new CultureHandler());

            config.Formatters.Add(new SirenJsonMediaTypeFormatter());
        }
    }
}