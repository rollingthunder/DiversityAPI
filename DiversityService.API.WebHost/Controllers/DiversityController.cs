namespace DiversityService.API.Controllers
{
    using DiversityService.API.Results;
    using System.Web.Http;
    using System.Web.Http.Routing;

    public abstract class DiversityController : ApiController
    {
        protected SeeOtherAtRouteResult SeeOtherAtRoute(string routeName, object routeValues)
        {
            return new SeeOtherAtRouteResult(routeName, new HttpRouteValueDictionary(routeValues), this);
        }
    }
}
