namespace DiversityService.API.WebHost.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Web.Http;
    using System.Web.Http.Results;
    using System.Web.Http.Routing;

    public abstract class DiversityController : ApiController
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

        protected SeeOtherRouteNegotiatedContentResult<T> SeeOtherAtRoute<T>(string routeName, object routeValues, T content)
        {
            return new SeeOtherRouteNegotiatedContentResult<T>(routeName, new HttpRouteValueDictionary(routeValues), content, this);
        }


    }
}
