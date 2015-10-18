namespace DiversityService.API.Controllers
{
    using System;
    using System.Net.Http;
    using System.Web.Http;
    using DiversityService.API.Results;
    using Tavis;
    using Tavis.Home;
    using Tavis.IANA;
    using Tavis.UriTemplates;

    [AllowAnonymous]
    [RoutePrefix("Home")]
    public class ApiHomeController : ApiController
    {
        private readonly HomeDocument document;

        public ApiHomeController()
        {
            document = new HomeDocument();

            // Account 
            var account = new Link()
            {
                Relation = Relations.ACCOUNT,
                Target = new Uri(Route.PrefixAccount, UriKind.Relative)
            };
            document.AddResource(account);

            // Series 
            var series = new Link()
            {
                Relation = Relations.SERIES_SINGLE,
                Template = new UriTemplate("series/{id}"),
            };

            var allowedMethods = new AllowHint();
            allowedMethods.AddMethod(HttpMethod.Get);
            allowedMethods.AddMethod(HttpMethod.Post);
            series.AddHint(allowedMethods);

            document.AddResource(series);
        }

        [HttpGet]
        [Route]
        public IHttpActionResult Get()
        {
            return Home(document);
        }

        private JsonHomeResult Home(HomeDocument doc)
        {
            return new JsonHomeResult(doc);
        }
    }
}