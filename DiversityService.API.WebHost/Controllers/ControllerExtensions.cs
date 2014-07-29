namespace DiversityService.API.WebHost.Controllers
{
    using DiversityService.API.Resources;
    using System;
    using System.Collections.Generic;
    using System.Net;
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Web.Http;
    using System.Web.Http.Results;
    using System.Web.Http.Routing;
    using System.Web.Mvc;

    public static class ControllerExtensions
    {

        public class SeeOtherRouteNegotiatedContentResult<T> : CreatedAtRouteNegotiatedContentResult<T>
        {
            public SeeOtherRouteNegotiatedContentResult(string routeName, IDictionary<string, object> routeValues, T content, ApiController controller)
                : base(routeName, routeValues, content, controller)
            {

            }
            public override async Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken)
            {
                var response = await base.ExecuteAsync(cancellationToken);
                response.StatusCode = HttpStatusCode.SeeOther;
                return response;
            }
        }

        public static SeeOtherRouteNegotiatedContentResult<T> SeeOtherAtRoute<T>(this ApiController controller, string routeName, object routeValues, T content)
        {
            return new SeeOtherRouteNegotiatedContentResult<T>(routeName, new HttpRouteValueDictionary(routeValues), content, controller);
        }
    }
}