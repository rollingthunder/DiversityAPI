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

    public class MapperModule : NinjectModule
    {
        public override void Load()
        {
            Mapper.Initialize(MapperConfig.Configure);

            Bind<IMappingEngine>()
                .ToConstant(Mapper.Engine);

            Bind<IMappingService>()
                .To<AutoMapperMappingService>()
                .InSingletonScope();
        }
    }
}