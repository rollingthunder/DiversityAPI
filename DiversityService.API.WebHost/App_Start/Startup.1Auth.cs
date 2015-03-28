using DiversityService.API.WebHost.Models;
using DiversityService.API.WebHost.Providers;
using Microsoft.AspNet.Identity;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OAuth;
using Owin;
using System;
using System.Configuration;

namespace DiversityService.API.WebHost
{
    public partial class Startup
    {
        public static OAuthAuthorizationServerOptions OAuthOptions { get; private set; }

        public static string PublicClientId { get; private set; }

        // For more information on configuring authentication, please visit http://go.microsoft.com/fwlink/?LinkId=301864
        public void ConfigureAuth(IAppBuilder app)
        {
            // Configure the db context and user manager to use a single instance per request
            app.CreatePerOwinContext(ApplicationDbContext.Create);
            app.CreatePerOwinContext<ApplicationUserManager>(ApplicationUserManager.Create);

            // Enable the application to use a cookie to store information for the signed in user
            // and to use a cookie to temporarily store information about a user logging in with a third party login provider
            app.UseExternalSignInCookie(DefaultAuthenticationTypes.ExternalCookie);

            // Configure the application for OAuth based flow
            PublicClientId = "self";
            OAuthOptions = new OAuthAuthorizationServerOptions
            {
                TokenEndpointPath = new PathString("/Token"),
                Provider = new ApplicationOAuthProvider(PublicClientId),
                AuthorizeEndpointPath = new PathString("/api/Account/ExternalLogin"),
                AccessTokenExpireTimeSpan = TimeSpan.FromDays(14),
                AllowInsecureHttp = false
            };

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
        public void ConfigureTestAuth(IAppBuilder app)
        {
        }
    }
}