namespace DiversityService.API.Controllers
{
    using DiversityService.API.Results;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Web.Http;
    using Tavis;
    using Tavis.Home;
    using Tavis.IANA;
    using Tavis.UriTemplates;

    [AllowAnonymous]
    [RoutePrefix("Home")]
    public class ApiHomeController : ApiController
    {
        private readonly HomeDocument Document;

        public ApiHomeController()
        {
            Document = new HomeDocument();

            // Account
            var account = new Link()
            {
                Relation = Relations.ACCOUNT,
                Target = new Uri(Route.PREFIX_ACCOUNT, UriKind.Relative)
            };
            Document.AddResource(account);

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

            Document.AddResource(series);
        }

        [HttpGet]
        [Route]
        public IHttpActionResult Get()
        {
            return Home(Document);
        }

        private JsonHomeResult Home(HomeDocument doc)
        {
            return new JsonHomeResult(doc);
        }
    }
}