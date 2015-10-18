namespace DiversityService.API.Filters
{
    using System.Net;
    using System.Net.Http;
    using System.Web.Http.Controllers;
    using System.Web.Http.Filters;
    using System.Web.Http.Routing;
    using DiversityService.API.Services;

    public class CollectionAPIAttribute : ActionFilterAttribute, IRoutePrefix
    {
        private readonly string prefix;

        public CollectionAPIAttribute(string routePrefix)
        {
            prefix = string.Format("{0}{1}{2}", CollectionAPI.ApiPrefix, CollectionAPI.CollectionPrefix, routePrefix);
        }

        public string Prefix
        {
            get { return prefix; }
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