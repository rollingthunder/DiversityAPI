namespace DiversityService.API.Services
{
    using System.Net.Http;
    using System.Web;
    using DiversityService.API.Model.Internal;

    public static class BackendCredentialsContextExtensions
    {
        private const string BackendCredentialsKey = "backend_creds";

        public static CollectionServerLogin GetBackendCredentials(this HttpRequestMessage request)
        {
            var requestContext = request.GetOwinContext();
            return requestContext.Get<CollectionServerLogin>(BackendCredentialsKey);
        }

        public static void SetBackendCredentials(this HttpRequestMessage request, CollectionServerLogin ctx)
        {
            var requestContext = request.GetOwinContext();
            requestContext.Set<CollectionServerLogin>(BackendCredentialsKey, ctx);
        }
    }
}