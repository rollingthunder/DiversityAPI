namespace DiversityService.API.Results
{
    using System.Collections.Generic;
    using System.Net;
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Web.Http;
    using System.Web.Http.Results;

    public class SeeOtherAtRouteResult : RedirectToRouteResult
    {
        public SeeOtherAtRouteResult(string routeName, IDictionary<string, object> routeValues, ApiController controller) 
            : base(routeName, routeValues, controller)
        {
        }

        public override async Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken)
        {
            var response = await base.ExecuteAsync(cancellationToken);

            response.StatusCode = HttpStatusCode.SeeOther;

            return response;
        }
    }
}