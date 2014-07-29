namespace DiversityService.API.WebHost
{
    using AutoMapper;
    using DiversityService.API.WebHost.Models;
    using DiversityService.API.WebHost.Services;
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

    public class ServiceModule : NinjectModule
    {
        private class RepositoryFactory : IRepositoryFactory
        {
            private readonly IKernel Kernel;
            public RepositoryFactory(IKernel kernel)
            {
                this.Kernel = kernel;
            }

            IRepository<T> IRepositoryFactory.Get<T>()
            {
                return Kernel.Get<IRepository<T>>();
            }
        }

        public override void Load()
        {
            Bind<IUnitOfWork>()
                .To<UnitOfWork>()
                .InRequestScope();

            Bind(typeof(IRepository<>))
                .To(typeof(Repository<>))
                .InRequestScope();

            Bind<IMappingEngine>()
                .ToConstant(Mapper.Engine);

            Bind<ApplicationUserManager>()
                .ToMethod(ctx =>
                {
                    return HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>();
                });

            Bind<ISecureDataFormat<AuthenticationTicket>>()
                .ToMethod(_ => Startup.OAuthOptions.AccessTokenFormat)
                .InSingletonScope();
                

            // TODO Register
            Bind<IContext>().ToMethod((ctx) =>
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

            Bind<IRepositoryFactory>()
                .To<RepositoryFactory>();
            
        }
    }
}