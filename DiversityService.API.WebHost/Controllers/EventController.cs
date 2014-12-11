namespace DiversityService.API.Controllers
{
    using AutoMapper;
    using DiversityService.API.Filters;
    using DiversityService.API.Model;
    using DiversityService.API.Services;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Web.Http;

    [CollectionAPI("event")]
    public class EventController : DiversityController
    {
        private readonly IMappingService Mapper;

        private IStore<Collection.Event, int> EventStore
        {
            get
            {
                return Request.GetCollectionContext().Events;
            }
        }

        public EventController(

            IMappingService mapper
            )
        {
            this.Mapper = mapper;
        }

        [Route]
        public async Task<IHttpActionResult> Get()
        {
            var allEvents = await EventStore.GetQueryableAsync();

            allEvents = allEvents.OrderBy(x => x.Id);

            var query = Mapper.Project<Collection.Event, Event>(allEvents);

            return Paged(query);
        }

        [Route]
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

        [Route]
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
        public async Task<IHttpActionResult> EventsForSeries(int? seriesId)
        {
            var allEvents = await EventStore.GetQueryableAsync();

            var eventsForSeries = from ev in allEvents
                                  where ev.SeriesID == seriesId
                                  select ev;

            var query = Mapper.Project<Collection.Event, Event>(eventsForSeries);

            return Paged(query);
        }
    }
}