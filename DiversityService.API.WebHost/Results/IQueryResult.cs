namespace DiversityService.API.Results
{
    using System.Linq;
    using System.Web.Http;

    public interface IQueryResult<T> : IHttpActionResult
    {
        IQueryable<T> Query { get; }
    }
}