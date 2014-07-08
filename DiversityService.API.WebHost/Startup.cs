using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(DiversityService.API.WebHost.Startup))]

namespace DiversityService.API.WebHost
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            

            ConfigureAuth(app);
        }
    }
}
