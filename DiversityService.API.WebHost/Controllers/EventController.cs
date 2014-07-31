namespace DiversityService.API.WebHost.Controllers
{
    using AutoMapper;
    using AutoMapper.QueryableExtensions;
    using DiversityService.API.Model;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Web.Http;
    using System.Web.Http.OData;
    using System.Text;
    using System.Web;
    
    public class EventController : DiversityController
    {
        private readonly IRepositoryFactory Repository;
        private readonly IMappingEngine Mapper;

        public EventController(
            IRepositoryFactory repo,
            AutoMapper.IMappingEngine mapper
            )
        {
            this.Repository = repo;
            this.Mapper = mapper;
        }

        // GET: api/Event
        [EnableQuery(PageSize = 20)]  
        public IQueryable<Event> Get()
        {
            var Events = this.Repository.Get<Collection.Event>();

            return Events.All
                .Project(Mapper)
                .To<Event>();
        }

        // GET: api/Event/5
        public Event Get(int id)
        {
            return null;
        }

        // POST: api/Event
        public void Post([FromBody]EventBindingModel value)
        {
        }

        // PUT: api/Event/5
        public void Put(int id, [FromBody]Event value)
        {
        }

        [Route(Route.PREFIX_SERIES + "{seriesId}/events", Name="EventsForSeries")]
        [HttpGet]
        public HttpResponseMessage EventsForSeries(int seriesId)
        {
            var response = Request.CreateResponse(HttpStatusCode.SeeOther);

            var uriBuilder = new StringBuilder();

            uriBuilder.Append(Url.Link(Route.NAME_DEFAULT_API, new { controller = Route.EVENT_CONTROLLER }));

            var query = Request.RequestUri.ParseQueryString();

            var seriesFilter = string.Format("SeriesId eq {0}", seriesId);

            var filterQuery = query["$filter"];
            if(filterQuery != null)
            {
                var newFilter = string.Format("{0} and {1}", filterQuery, seriesFilter);
                query.Set("$filter", newFilter);
            }
            else
            {
                query.Add("$filter", seriesFilter);
            }

            var ammendedQuery = HttpUtility.UrlDecode(query.ToString());

            uriBuilder.Append('?');
            uriBuilder.Append(ammendedQuery);

            response.Headers.Location = new Uri(uriBuilder.ToString());

            return response;
        }
    }
}
