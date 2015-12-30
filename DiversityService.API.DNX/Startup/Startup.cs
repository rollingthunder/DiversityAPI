﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Authentication.JwtBearer;
using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Hosting;
using Microsoft.AspNet.Mvc.Controllers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Swashbuckle.SwaggerGen;
using Microsoft.AspNet.Owin;
using System.IdentityModel.Tokens.Jwt;
using IdentityServer3.Core.Models;
using Microsoft.AspNet.Authentication.OpenIdConnect;
using System.Security.Claims;
using Thinktecture.IdentityModel.Client;
using Microsoft.AspNet.Authentication;

namespace DiversityService.API.DNX
{
    public partial class Startup
    {
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

        public IConfigurationRoot Configuration { get; set; }

        // This method gets called by the runtime. Use this method to add services to the container
        public void ConfigureServices(IServiceCollection services)
        {
            // Add framework services.
            services.AddApplicationInsightsTelemetry(Configuration);

            services.AddMvc();

            services.AddSwaggerGen();
            services.ConfigureSwaggerDocument( opt =>
            {
                opt.SingleApiVersion(new Info()
                {
                    Version = "v1",
                    Title = "DiversityAPI",
                    Description = "A REST API to the DiversityCollection data store",
                    TermsOfService = ""
                });
            });
            services.ConfigureSwaggerSchema(opt =>
            {
                opt.DescribeAllEnumsAsStrings = true;
            });

            services.AddSingleton<IControllerActivator, NinjectControllerActivator>(ConfigureNinject);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            app.UseIISPlatformHandler();

            app.UseApplicationInsightsRequestTelemetry();

            app.UseApplicationInsightsExceptionTelemetry();

            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

            app.UseJwtBearerAuthentication(options =>
            {
                options.Authority = Configuration["Authentication:EndpointUrl"];
                options.RequireHttpsMetadata = false;

                options.Audience = string.Format("{0}/resources", Configuration["Authentication:EndpointUrl"]);
                options.AutomaticAuthenticate = false;

                options.AuthenticationScheme = "Jwt";
            });

            app.UseCookieAuthentication(options =>
            {
                options.AuthenticationScheme = "Cookies";
                options.AutomaticAuthenticate = true;
            });

            app.UseOpenIdConnectAuthentication(options =>
            {
                options.AutomaticChallenge = true;
                options.AuthenticationScheme = "Oidc";
                options.SignInScheme = "Jwt";

                options.Authority = Configuration["Authentication:EndpointUrl"];
                options.RequireHttpsMetadata = false;

                options.ClientId = "diversityapi";
                options.ResponseType = "id_token token";

                options.Scope.Add("openid");
                options.Scope.Add("email");
                options.Scope.Add("diversityapi");

                //options.Events = new OpenIdConnectEvents()
                //{
                //    OnAuthorizationResponseReceived = async n =>
                //    {
                //        var nid = new ClaimsIdentity(
                //            n.AuthenticationTicket.AuthenticationScheme,
                //            ClaimTypes.GivenName,
                //            ClaimTypes.Role);

                //        // get userinfo data
                //        var userInfoClient = new UserInfoClient(
                //            new Uri(n.Options.Authority + "/connect/userinfo"),
                //            n.ProtocolMessage.AccessToken);

                //        var userInfo = await userInfoClient.GetAsync();
                //        userInfo.Claims.ToList().ForEach(ui => nid.AddClaim(new Claim(ui.Item1, ui.Item2)));

                //        // keep the id_token for logout
                //        nid.AddClaim(new Claim("id_token", n.ProtocolMessage.IdToken));

                //        // add access token for sample API
                //        nid.AddClaim(new Claim("access_token", n.ProtocolMessage.AccessToken));

                //        // keep track of access token expiration
                //        nid.AddClaim(new Claim("expires_at", DateTimeOffset.Now.AddSeconds(int.Parse(n.ProtocolMessage.ExpiresIn)).ToString()));

                //        // add some other app specific claim
                //        nid.AddClaim(new Claim("app_specific", "some data"));

                //        n.AuthenticationTicket = new AuthenticationTicket(
                //            new ClaimsPrincipal(nid),
                //            n.AuthenticationTicket.Properties,
                //            n.AuthenticationTicket.AuthenticationScheme);
                //    }
                //};
            });

            app.UseClaimsTransformation(principal =>
            {
            });

            app.UseStaticFiles();
            app.UseDeveloperExceptionPage();
            app.UseMvc();


            app.UseSwaggerGen();
            app.UseSwaggerUi();
        }

        // Entry point for the application.
        public static void Main(string[] args) => WebApplication.Run<Startup>(args);
    }
}
