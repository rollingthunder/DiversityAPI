namespace DiversityService.API.Results
{
    using System.Net;
    using System.Net.Http;
    using System.Web.Http.Results;
    using Tavis.Home;

    public class JsonHomeResult : ResponseMessageResult
    {
        public JsonHomeResult(HomeDocument document)
            : base(new HttpResponseMessage(HttpStatusCode.OK) { Content = new HomeContent(document) })
        {
        }
    }
}