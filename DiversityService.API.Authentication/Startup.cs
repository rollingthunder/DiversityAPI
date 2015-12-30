using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Hosting;
using Microsoft.AspNet.Http;
using Microsoft.Extensions.DependencyInjection;
using IdentityServer3.Core.Configuration;
using System.Security.Cryptography.X509Certificates;
using Microsoft.Extensions.PlatformAbstractions;
using Microsoft.Extensions.Logging;
using Owin;
using System;
using Microsoft.AspNet.Authentication.MicrosoftAccount;
using Microsoft.Extensions.Configuration;
using Microsoft.Owin.Security.MicrosoftAccount;

namespace DiversityService.API.Authentication
{
    public class Startup
    {
        private readonly IConfigurationRoot Configuration;

        public Startup(IHostingEnvironment env)
        {
            // Set up configuration sources.
            var builder = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json");

            if (env.IsDevelopment())
            {
                // This will push telemetry data through Application Insights pipeline faster, allowing you to view results immediately.
                builder.AddApplicationInsightsSettings(developerMode: true);

                builder.AddUserSecrets();
            }

            builder.AddEnvironmentVariables();
            Configuration = builder.Build().ReloadOnChanged("appsettings.json");
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDataProtection();
        }

        public void Configure(IApplicationBuilder app, IApplicationEnvironment env, ILoggerFactory loggerFactory)
        {
            //Log.Logger = new LoggerConfiguration()
            //    .MinimumLevel.Debug()
            //    .WriteTo.LiterateConsole()
            //    .CreateLogger();

            //loggerFactory.AddConsole();
            //loggerFactory.AddDebug();

            app.UseIISPlatformHandler();


            var certFile = env.ApplicationBasePath + $"{System.IO.Path.DirectorySeparatorChar}authority.pfx";

            var idsrvOptions = new IdentityServerOptions
            {
                Factory = new IdentityServerServiceFactory()
                                .UseInMemoryUsers(Users.Get())
                                .UseInMemoryClients(Clients.Get())
                                .UseInMemoryScopes(Scopes.Get()),

                SigningCertificate = new X509Certificate2(certFile),
                RequireSsl = false,
                AuthenticationOptions = new AuthenticationOptions()
                {
                    IdentityProviders = ExternalIdentities(app),
                    
                }
            };

            app.UseIdentityServer(idsrvOptions);
        }

        Action<IAppBuilder, string> ExternalIdentities(IApplicationBuilder app)
        {
            return (OwinApp, SignInScheme) =>
            {
                OwinApp.UseMicrosoftAccountAuthentication(new MicrosoftAccountAuthenticationOptions()
                {
                    SignInAsAuthenticationType = SignInScheme,
                    AuthenticationType = "Microsoft",
                    ClientId = Configuration["Authentication:Microsoft:AppId"],
                    ClientSecret = Configuration["Authentication:Microsoft:AppSecret"]
                });
            };
        }
        public static void Main(string[] args) => WebApplication.Run<Startup>(args);
    }
}
