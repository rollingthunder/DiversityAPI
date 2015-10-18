namespace DiversityService.API.Services
{
    using System.Net.Http;
    using System.Web;

    public static class CollectionContextExtensions
    {
        private const string COLLECTION_CONTEXT_KEY = "coll_context";

        public static IFieldDataContext GetCollectionContext(this HttpRequestMessage request)
        {
            var requestContext = request.GetOwinContext();
            return requestContext.Get<IFieldDataContext>(COLLECTION_CONTEXT_KEY);
        }

        public static void SetCollectionContext(this HttpRequestMessage request, IFieldDataContext ctx)
        {
            var requestContext = request.GetOwinContext();
            requestContext.Set<IFieldDataContext>(COLLECTION_CONTEXT_KEY, ctx);
        }
    }
}