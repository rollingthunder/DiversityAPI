namespace DiversityService.API.WebHost
{
    using Microsoft.Owin;
    using Owin;

    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
            ConfigureWebApi(app);
        }
    }
}