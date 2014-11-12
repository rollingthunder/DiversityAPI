namespace DiversityService.API.WebHost
{
    using AutoMapper;
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
    using System.Security.Principal;
    using System.Web;

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

            Bind<IPrincipal>()
                .ToMethod((ctx) => HttpContext.Current.User)
                .InRequestScope();

            Bind<ApplicationUser>()
                .ToMethod((ctx) =>
                {
                    var User = ctx.Kernel.Get<IPrincipal>();
                    var UserManager = ctx.Kernel.Get<ApplicationUserManager>();
                    var userId = User.Identity.GetUserId();
                    var appUser = UserManager.FindById(userId);
                    return appUser;
                }).InRequestScope();

            //Bind<CollectionContext>()
            //    .ToMethod((ctx) =>
            //    {
            //        var appUser = ctx.Kernel.Get<ApplicationUser>();
            //        var servers = ctx.Kernel.Get<IEnumerable<InternalCollectionServer>>();
            //        var server = servers.Single(x => x.Id == appUser.CollectionId);

            //        string connectionString = ConfigurationManager.ConnectionStrings["Collection"].ConnectionString;
            //        var withCredentials = string.Format(connectionString, server.Address, server.Port, server.Catalog, appUser.BackendUser, appUser.BackendPassword);
            //        return new CollectionContext(withCredentials);
            //    })
            //    .InRequestScope();
        }
    }
}