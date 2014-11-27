namespace DiversityService.API.Controllers
{
    using DiversityService.API.Model;
    using DiversityService.API.Results;
    using DiversityService.API.Services;
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Web.Http;
    using System.Web.Http.Routing;

    public abstract class DiversityController : ApiController
    {
        protected SeeOtherAtRouteResult SeeOtherAtRoute(string routeName, object routeValues)
        {
            return new SeeOtherAtRouteResult(routeName, new HttpRouteValueDictionary(routeValues), this);
        }

        protected async Task<SeeOtherAtRouteResult> RedirectToExisting<T, TKey>(IStore<T, TKey> This, Guid rowGuid)
             where T : IGuidIdentifiable, IIdentifiable
        {
            var existingRows = await This.GetAsync(
                x => x.RowGUID == rowGuid
                );
            var existing = existingRows.SingleOrDefault();

            if (existing != null)
            {
                return SeeOtherAtRoute(Route.DEFAULT_API, Route.GetById(existing));
            }
            return null;
        }
    }
}