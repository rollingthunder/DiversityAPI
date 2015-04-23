namespace DiversityService.API.Client
{
    using Newtonsoft.Json.Linq;
    using Splat;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Linq;
    using System.Net.Http;
    using System.Text;
    using System.Threading.Tasks;

    public class APIAuthentication : IEnableLogger
    {
        public const string PROVIDER_NAME_MICROSOFT = "Microsoft";

        public const string CORRELATION_COOKIE_NAME = ".AspNet.Correlation.";

        public const string URI_EXTERNAL_LOGINS = "/api/Account/ExternalLogins?returnUrl=%2F&generateState=true";

        public HttpClient Client { get; private set; }

        public APIAuthentication(Uri baseAddress, HttpMessageHandler handler = null)
        {
            handler = handler ?? new HttpClientHandler();

            // Do not use Cookie functionality from HttpClientHandler
            // because we want to be able to work with any (test) Handler passed in.
            handler = new CookieHandler() { InnerHandler = handler };

            Client = new HttpClient(handler)
            {
                BaseAddress = baseAddress
            };
        }

        public async Task<string> GetLoginUriAsync(string providerName = PROVIDER_NAME_MICROSOFT)
        {
            try
            {
                var loginsResponse = await Client.GetAsync(URI_EXTERNAL_LOGINS);

                var logins = await loginsResponse.Content.ReadAsAsync<JArray>();

                var url = (from login in logins
                           where login["Name"].ToString() == providerName
                           select login["Url"].ToString())
                        .FirstOrDefault();

                if (url == null)
                {
                    return null;
                }

                var redirect = await Client.GetAsync(url);

                return redirect.Headers.Location.AbsoluteUri;
            }
            catch (Exception ex)
            {
                this.Log().ErrorException("GetLoginUriAsync", ex);
            }

            return null;
        }

        public async Task<string> AuthenticateReturnURLAsync(string returnUrl)
        {
            try
            {
                var externalSigninResponse = await Client.GetAsync(returnUrl);

                var signinUri = externalSigninResponse.Headers.Location.AbsoluteUri;

                var signinResponse = await Client.GetAsync(signinUri);

                return signinResponse.Headers.Location.Fragment;
            }
            catch (Exception ex)
            {
                this.Log().ErrorException("AuthenticateReturnURLAsync", ex);
            }

            return null;
        }
    }
}