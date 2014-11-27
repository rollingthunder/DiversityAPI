namespace DiversityService.API.WebHost
{
    using Microsoft.Owin;
    using Owin;

    [assembly: OwinStartup(typeof(DiversityService.API.WebHost.Startup))]
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}