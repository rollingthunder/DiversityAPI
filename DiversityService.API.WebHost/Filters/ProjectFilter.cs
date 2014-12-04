namespace DiversityService.API.Filters
{
    using DiversityService.API.Services;
    using System.Net;
    using System.Net.Http;
    using System.Web.Http.Controllers;

    public class ProjectAttribute : CollectionAPIAttribute
    {
        public ProjectAttribute(string routePrefix)
            : base(string.Format("{0}/{1}", CollectionAPI.PROJECT_PREFIX, routePrefix))
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