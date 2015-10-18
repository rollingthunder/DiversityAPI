namespace DiversityService.API.Services
{
    using System.Net.Http;
    using System.Web;

    public static class CollectionContextExtensions
    {
        private const string CollectionContextKey = "coll_context";

        public static IFieldDataContext GetCollectionContext(this HttpRequestMessage request)
        {
            var requestContext = request.GetOwinContext();
            return requestContext.Get<IFieldDataContext>(CollectionContextKey);
        }

        public static void SetCollectionContext(this HttpRequestMessage request, IFieldDataContext ctx)
        {
            var requestContext = request.GetOwinContext();
            requestContext.Set<IFieldDataContext>(CollectionContextKey, ctx);
        }
    }
}