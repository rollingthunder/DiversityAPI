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

    [AllowAnonymous]
    public class ApiHomeController : ApiController
    {
        private readonly HomeDocument Document;

        public ApiHomeController()
        {
            Document = new HomeDocument();

            // Series
            var series = new Link()
            {
                Relation = Relations.SERIES_SINGLE,                
                Target = new Uri("series/{id}", UriKind.Relative)
            };
            series.SetParameter("id", "", Parameters.SERIES_ID);

            var allowedMethods = new AllowHint();
            allowedMethods.AddMethod(HttpMethod.Get);
            allowedMethods.AddMethod(HttpMethod.Post);
            series.AddHint(allowedMethods);

            Document.AddResource(series);
        }

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
