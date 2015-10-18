namespace DiversityService.API.Results
{
    using System.Web.Http;

    public interface IChainedResult
    {
        IHttpActionResult InnerResult { get; }
    }
}