namespace DiversityService.API.WebHost
{
    using Microsoft.Owin;
    using Owin;

    public partial class Startup
    {
        public virtual void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
            ConfigureNinject(app);
            ConfigureWebApi(app);
        }
    }

    public partial class TestStartup : Startup
    {
        public override void Configuration(IAppBuilder app)
        {
            ConfigureTestAuth(app);
            ConfigureNinject(app);
            ConfigureWebApi(app);
        }
    }
}