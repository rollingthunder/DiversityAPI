namespace DiversityService.API.Client
{
    using Newtonsoft.Json.Linq;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Linq;
    using System.Net.Http;
    using System.Net.Http.Formatting;
    using System.Net.Http.Headers;
    using System.Text;
    using System.Threading.Tasks;
    using Tavis.Home;

    public class APIBrowser
    {
        public const string HOME_URI = "/Home";

        public HttpClient Client { get; private set; }

        public HomeDocument Home { get; private set; }

        public APIBrowser(HttpClient client)
        {
            Contract.Requires<ArgumentNullException>(client != null);

            Client = client;
        }

        public async Task InitializeAsync()
        {
            var homeResponse = await Client.GetAsync(HOME_URI);

            homeResponse.EnsureSuccessStatusCode();

            Home = HomeDocument.Parse(await homeResponse.Content.ReadAsStreamAsync());
        }

        public async Task<Uri> GetAccountUriAsync()
        {
            RequireInitialized();

            return (from res in Home.Resources
                    where res.Relation == Relations.ACCOUNT
                    select res.Target).FirstOrDefault();
        }

        private void RequireInitialized()
        {
            Contract.Requires<InvalidOperationException>(Home != null, "not initialized");
        }
    }
}