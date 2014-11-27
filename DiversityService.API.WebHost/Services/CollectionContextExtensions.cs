namespace DiversityService.API.Services
{
    using DiversityService.API.Services;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using System.Web;

    public static class CollectionContextExtensions
    {
        private const string COLLECTION_CONTEXT_KEY = "coll_context";

        public static IContext GetCollectionContext(this HttpRequestMessage request)
        {
            var requestContext = request.GetOwinContext();
            return requestContext.Get<IContext>(COLLECTION_CONTEXT_KEY);
        }

        public static void SetCollectionContext(this HttpRequestMessage request, IContext ctx)
        {
            var requestContext = request.GetOwinContext();
            requestContext.Set<IContext>(COLLECTION_CONTEXT_KEY, ctx);
        }
    }
}