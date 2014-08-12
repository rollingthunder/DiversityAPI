namespace DiversityService.API.WebHost
{
    using AutoMapper;
    using DiversityService.API.WebHost.Models;
    using DiversityService.API.Services;
    using Microsoft.AspNet.Identity;
    using Ninject;
    using Ninject.Modules;
    using Ninject.Web.Common;
    using System.Configuration;
    using System.Security.Principal;
    using System.Web;
    using System.Linq;
    using Microsoft.AspNet.Identity.Owin;
    using Microsoft.Owin.Security;
    using Microsoft.Owin.Security.Cookies;
    using DiversityService.API.Model;
    using System.Collections.Generic;
    using Ninject.Extensions.Factory;

    public class ServiceModule : NinjectModule
    {
        public override void Load()
        {

            Bind<ISeriesStore>().To<SeriesStore>()
                .InRequestScope();

            Bind<IMappingService>()
                .To<AutoMapperMappingService>()
                .InSingletonScope();

            Bind<ApplicationUserManager>()
                .ToMethod(ctx =>
                {
                    return HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>();
                });

            Bind<ISecureDataFormat<AuthenticationTicket>>()
                .ToMethod(_ => Startup.OAuthOptions.AccessTokenFormat)
                .InSingletonScope();
                

            // TODO Register
            Bind<CollectionContext>().ToMethod((ctx) =>
            {
                var User = HttpContext.Current.User;
                var UserManager = ctx.Kernel.Get<ApplicationUserManager>();
                var userId = User.Identity.GetUserId();
                var appUser = UserManager.FindById(userId);

                var servers = ctx.Kernel.Get<IEnumerable<InternalCollectionServer>>();
                var server = servers.Single(x => x.Id == appUser.CollectionId);

                string connectionString = ConfigurationManager.ConnectionStrings["Collection"].ConnectionString;
                var withCredentials = string.Format(connectionString, server.Address, server.Port, server.Catalog, appUser.BackendUser, appUser.BackendPassword);
                return new CollectionContext(withCredentials);
            }).InRequestScope();            
        }
    }
}