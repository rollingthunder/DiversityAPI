namespace DiversityService.API.Services
{
    using DiversityService.API.Model.Internal;
    using System.Net.Http;
    using System.Web;

    public static class BackendCredentialsContextExtensions
    {
        private const string BACKEND_CREDENTIALS_KEY = "backend_creds";

        public static CollectionServerLogin GetBackendCredentials(this HttpRequestMessage request)
        {
            var requestContext = request.GetOwinContext();
            return requestContext.Get<CollectionServerLogin>(BACKEND_CREDENTIALS_KEY);
        }

        public static void SetBackendCredentials(this HttpRequestMessage request, CollectionServerLogin ctx)
        {
            var requestContext = request.GetOwinContext();
            requestContext.Set<CollectionServerLogin>(BACKEND_CREDENTIALS_KEY, ctx);
        }
    }
}