namespace DiversityService.API.Client
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Linq;
    using System.Net.Http;
    using System.Text;
    using System.Threading.Tasks;

    public class APIAuthentication
    {
        public const string PROVIDER_NAME_MICROSOFT = "Microsoft";

        public const string CORRELATION_COOKIE_NAME = ".AspNet.Correlation.";

        public const string URI_EXTERNAL_LOGINS = "/api/Account/ExternalLogins?returnUrl=%2F&generateState=true";

        public HttpClient Client { get; private set; }

        public APIAuthentication(Uri baseAddress, HttpMessageHandler handler = null)
        {
            Client = new HttpClient()
            {
                BaseAddress = baseAddress
            };
        }

        public async Task<string> GetLoginUriAsync(string providerName = PROVIDER_NAME_MICROSOFT)
        {
            var loginsResponse = Client.GetAsync(URI_EXTERNAL_LOGINS);

            return "";
        }
    }
}