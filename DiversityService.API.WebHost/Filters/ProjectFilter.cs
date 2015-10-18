namespace DiversityService.API.Filters
{
    using System.Net;
    using System.Net.Http;
    using System.Web.Http.Controllers;
    using DiversityService.API.Services;

    public class ProjectAPIAttribute : CollectionAPIAttribute
    {
        public ProjectAPIAttribute(string routePrefix)
            : base(string.Format("{0}/{1}", CollectionAPI.ProjectPrefix, routePrefix))
        {
        }

        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            var request = actionContext.Request;
            var ctx = request.GetCollectionContext();

            if (ctx != null && ctx.ProjectId.HasValue)
            {
                base.OnActionExecuting(actionContext);
            }
            else
            {
                actionContext.Response = request.CreateErrorResponse(HttpStatusCode.Forbidden, "Collection Server and Project required");
            }
        }
    }
}