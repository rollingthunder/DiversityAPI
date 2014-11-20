namespace DiversityService.API.Filters
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Web;
    using System.Web.Http.Controllers;
    using System.Web.Http.Filters;

    public class RequireCollectionAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            var request = actionContext.Request;
            if (request.GetCollectionContext() != null)
            {
                base.OnActionExecuting(actionContext);
            }
            else
            {
                actionContext.Response = request.CreateErrorResponse(HttpStatusCode.NotFound, "Not Found");
            }
        }
    }
}