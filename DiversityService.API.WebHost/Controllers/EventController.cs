namespace DiversityService.API.Controllers
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
    using DiversityService.API.Services;
    using System.Threading.Tasks;

    public class EventController : DiversityController
    {
        private readonly IEventStore EventStore;
        private readonly IMappingService Mapper;

        public EventController(
            IEventStore repo,
            IMappingService mapper
            )
        {
            this.EventStore = repo;
            this.Mapper = mapper;
        }

        // GET: api/Event        
        public async Task<IQueryable<Event>> Get()
        {
            var allEvents = await EventStore.FindAsync();

            return Mapper.Project<Collection.Event, Event>(allEvents);
        }

        // GET: api/Event/5
        public async Task<IHttpActionResult> Get(int id)
        {
            var ev = await EventStore.FindAsync(id);

            if (ev == null)
            {
                return NotFound();
            }

            var dto = Mapper.Map<Event>(ev);

            return Ok(dto);
        }

        // POST: api/Event
        public async Task<IHttpActionResult> Post(EventBindingModel value)
        {
            var existing = (from ev in await EventStore.FindAsync()
                            where ev.RowGUID == value.TransactionGuid
                            select ev).SingleOrDefault();

            if (existing != null)
            {
                return SeeOtherAtRoute(Route.DEFAULT_API, Route.GetById(existing));
            }

            var newEvent = Mapper.Map<Collection.Event>(value);

            await EventStore.InsertAsync(newEvent);

            return CreatedAtRoute(Route.DEFAULT_API, Route.GetById(newEvent), newEvent.Id);
        }

        [Route(Route.PREFIX_SERIES + "{seriesId}/events", Name = "EventsForSeries")]
        [HttpGet]
        public async Task<IQueryable<Event>> EventsForSeries(int? seriesId)
        {
            var allEvents = await EventStore.FindAsync();

            var query = from ev in allEvents
                        where ev.SeriesID == seriesId
                        select ev;

            return Mapper.Project<Collection.Event, Event>(query);
        }
    }
}
