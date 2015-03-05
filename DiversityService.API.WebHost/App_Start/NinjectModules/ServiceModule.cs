namespace DiversityService.API.WebHost
{
    using AutoMapper;
    using DiversityService.API.Filters;
    using DiversityService.API.Model;
    using DiversityService.API.Services;
    using DiversityService.API.WebHost.Models;
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.Owin;
    using Microsoft.Owin.Security;
    using Microsoft.Owin.Security.Cookies;
    using Ninject;
    using Ninject.Extensions.Factory;
    using Ninject.Modules;
    using Ninject.Web.Common;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Linq;
    using System.Net.Http;
    using System.Security.Principal;
    using System.Web.Http.Controllers;

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

            Bind<ITaxa>()
                .To<TaxonService>();
        }
    }
}