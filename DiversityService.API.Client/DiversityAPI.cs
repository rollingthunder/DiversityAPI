namespace DiversityService.API.Client
{
    using System;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Threading.Tasks;

    public class DiversityAPI
    {
        public readonly APIBrowser Browser;
        public readonly HttpClient Client;

        public DiversityAPI(string access_token, APIBrowser browser)
        {
            Browser = browser;

            Client = Browser.Client;

            Client.DefaultRequestHeaders
                .Authorization = new AuthenticationHeaderValue("Bearer", access_token);
        }

        public async Task SetBackendCredentialsAsync(string user, string pass)
        {
            var credentials = new
            {
                BackendUser = user,
                BackendPassword = pass
            };

            var accountUri = await Browser.GetAccountUriAsync();

            var backendUri = accountUri + "Backend";

            var setResponse = await Client.PostAsJsonAsync(backendUri, credentials);

            setResponse.EnsureSuccessStatusCode();
        }
    }
}