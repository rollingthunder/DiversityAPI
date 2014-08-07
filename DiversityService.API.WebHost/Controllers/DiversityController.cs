namespace DiversityService.API.Controllers
{
    using DiversityService.API.Results;
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
        protected SeeOtherAtRouteResult SeeOtherAtRoute(string routeName, object routeValues)
        {
            return new SeeOtherAtRouteResult(routeName, new HttpRouteValueDictionary(routeValues), this);
        }
    }
}
