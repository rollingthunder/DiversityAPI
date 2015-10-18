namespace DiversityService.API.Client
{
    using System;
    using System.Diagnostics.Contracts;
    using System.Linq;
    using System.Net.Http;
    using System.Threading.Tasks;
    using Tavis.Home;

    public class APIBrowser
    {
        public const string HOME_URI = "Home";

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
            Contract.Requires<InvalidOperationException>(Home != null, "not initialized");

            return (from res in Home.Resources
                    where res.Relation == Relations.ACCOUNT
                    select res.Target).FirstOrDefault();
        }
    }
}