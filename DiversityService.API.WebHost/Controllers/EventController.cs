namespace DiversityService.API.Controllers
{
    using AutoMapper;
    using AutoMapper.QueryableExtensions;
    using DiversityService.API.Model;
    using DiversityService.API.Services;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Text;
    using System.Threading.Tasks;
    using System.Web;
    using System.Web.Http;

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
            var allEvents = await EventStore.GetQueryableAsync();

            return Mapper.Project<Collection.Event, Event>(allEvents);
        }

        // GET: api/Event/5
        public async Task<IHttpActionResult> Get(int id)
        {
            var ev = await EventStore.GetByIDAsync(id);

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
            var existing = await RedirectToExisting(EventStore, value.TransactionGuid);

            if (existing != null)
            {
                return existing;
            }

            var newEvent = Mapper.Map<Collection.Event>(value);

            await EventStore.InsertAsync(newEvent);

            return CreatedAtRoute(Route.DEFAULT_API, Route.GetById(newEvent), newEvent.Id);
        }

        [Route(Route.PREFIX_SERIES + "{seriesId}/events", Name = "EventsForSeries")]
        [HttpGet]
        public async Task<IQueryable<Event>> EventsForSeries(int? seriesId)
        {
            var allEvents = await EventStore.GetQueryableAsync();

            var query = from ev in allEvents
                        where ev.SeriesID == seriesId
                        select ev;

            return Mapper.Project<Collection.Event, Event>(query);
        }
    }
}