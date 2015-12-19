using System.Web.Http;
using WebActivatorEx;
using DiversityService.API.WebHost;

[assembly: PreApplicationStartMethod(typeof(SwaggerConfig), "Register")]

namespace DiversityService.API.WebHost
{
    public class SwaggerConfig
    {
        public static void Register()
        {
            var thisAssembly = typeof(SwaggerConfig).Assembly;

            Startup.ConfigureSwagger(GlobalConfiguration.Configuration);
        }
    }
}
