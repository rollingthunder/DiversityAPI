namespace DiversityService.API.WebHost
{
    using Ninject;
    using Ninject.Web.WebApi.OwinHost;
    using Owin;
    using System.Reflection;
    using System.Web.Http;

    public partial class Startup
    {
        public void ConfigureWebApi(IAppBuilder app)
        {
            var config = new HttpConfiguration();

            WebApiConfig.Register(Kernel, config);

            app.UseNinjectWebApi(config);
        }
    }
}