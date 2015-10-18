using DiversityService.API.Model;
using DiversityService.API.WebHost.Models;
using DiversityService.API.WebHost.Providers;
using Microsoft.AspNet.Identity;
using Microsoft.Owin;
using Microsoft.Owin.Security.DataHandler;
using Microsoft.Owin.Security.DataProtection;
using Microsoft.Owin.Security.OAuth;
using Owin;
using System;
using System.Configuration;
using System.Security.Claims;

namespace DiversityService.API.WebHost
{
    public partial class Startup
    {
        public static readonly string PublicClientId = "self";

        public static readonly OAuthAuthorizationServerOptions OAuthOptions = new OAuthAuthorizationServerOptions
        {
            TokenEndpointPath = new PathString("/Token"),
            Provider = new ApplicationOAuthProvider(PublicClientId),
            AuthorizeEndpointPath = new PathString("/api/Account/ExternalLogin"),
            AccessTokenExpireTimeSpan = TimeSpan.FromDays(14),
            AllowInsecureHttp = false
        };

        // For more information on configuring authentication, please visit http://go.microsoft.com/fwlink/?LinkId=301864
        public void ConfigureAuth(IAppBuilder app)
        {
            // Configure the db context and user manager to use a single instance per request
            app.CreatePerOwinContext(ApplicationDbContext.Create);
            app.CreatePerOwinContext<ApplicationUserManager>(ApplicationUserManager.Create);

            // Enable the application to use a cookie to store information for the signed in user
            // and to use a cookie to temporarily store information about a user logging in with a third party login provider
            app.UseExternalSignInCookie(DefaultAuthenticationTypes.ExternalCookie);

            // Enable the application to use bearer tokens to authenticate users
            app.UseOAuthBearerTokens(OAuthOptions);

            // Retrieve Client Id and Secret from AppSettings
            // If there are none, use the Test Application
            var clientId = ConfigurationManager.AppSettings["LiveClientId"] ?? "000000004C0FE46A";
            var clientSecret = ConfigurationManager.AppSettings["LiveClientSecret"] ?? "EarX-Ngmov1EebwO2Ia9CyLkvI9VJMhk";

            app.UseMicrosoftAccountAuthentication(
               clientId: clientId,
               clientSecret: clientSecret);
        }
    }

    public partial class TestStartup
    {
        public const string AuthorizationToken = "arstarstarstarstarstrst";

        public const string TestUserName = "test@user.com";
        public const string TestBackendUser = "Test";
        public const string TestBackendPass = "Pass";

        public void ConfigureTestAuth(IAppBuilder app)
        {
            // Default Data Fomat from Katana
            // Necessary to satisfy IOC
            IDataProtector dataProtecter = app.CreateDataProtector(
                    typeof(OAuthAuthorizationServerMiddleware).Namespace,
                    "Access_Token", "v1");
            OAuthOptions.AccessTokenFormat = new TicketDataFormat(dataProtecter);

            app.Use(async (ctx, next) =>
            {
                var req = ctx.Request;
                var auth = req.Headers.Get("Authorization");
                if (auth != null && auth.ToLower() == string.Format("bearer {0}", AuthorizationToken))
                {
                    var identity = CreateTestIdentity();
                    var principal = new ClaimsPrincipal(new[] { identity });

                    ctx.Authentication.User = principal;

                    req.Headers.Remove("Authorization");
                }

                await next();
            });
        }

        public static ClaimsIdentity CreateTestIdentity()
        {
            return new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Name, TestUserName),
                new BackendCredentialsClaim(TestBackendUser, TestBackendPass)
            }, OAuthDefaults.AuthenticationType);
        }
    }
}