namespace DiversityService.API.Filters
{
    using DiversityService.API.Services;
    using System.Net;
    using System.Net.Http;
    using System.Web.Http.Controllers;
    using System.Web.Http.Filters;
    using System.Web.Http.Routing;

    public class CollectionAPIAttribute : ActionFilterAttribute, IRoutePrefix
    {
        private readonly string _Prefix;

        public string Prefix
        {
            get { return _Prefix; }
        }

        public CollectionAPIAttribute(string routePrefix)
        {
            _Prefix = string.Format("{0}{1}{2}", CollectionAPI.API_PREFIX, CollectionAPI.COLLECTION_PREFIX, routePrefix);
        }

        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            var request = actionContext.Request;
            if (request.GetCollectionContext() != null)
            {
                base.OnActionExecuting(actionContext);
            }
            else
            {
                actionContext.Response = request.CreateErrorResponse(HttpStatusCode.Forbidden, "Valid Backend Credentials Required");
            }
        }
    }
}