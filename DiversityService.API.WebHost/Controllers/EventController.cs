namespace DiversityService.API.Controllers
{
    using AutoMapper;
    using DiversityService.API.Filters;
    using DiversityService.API.Model;
    using DiversityService.API.Services;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Web.Http;

    [CollectionAPI(Route.EVENT_CONTROLLER)]
    public class EventController : DiversityController, IFieldDataController<EventBindingModel>
    {
        private readonly IMappingEngine Mapper;

        private IStore<Collection.Event, int> EventStore
        {
            get
            {
                return Request.GetCollectionContext().Events;
            }
        }

        public EventController(
            IMappingEngine mapper
            )
            : base(mapper)
        {
            this.Mapper = mapper;
        }

        [Route]
        public async Task<IHttpActionResult> Get()
        {
            var allEvents = await EventStore.GetQueryableAsync();

            var query = allEvents.OrderBy(x => x.Id);

            return PagedAndMapped<Collection.Event, Event>(query);
        }

        [Route("{id}", Name = Route.EVENT_BYID)]
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
            var existing = await RedirectToExisting(EventStore, value.TransactionGuid, Route.EVENT_BYID);

            if (existing != null)
            {
                return existing;
            }

            var newEvent = Mapper.Map<Collection.Event>(value);

            await EventStore.InsertAsync(newEvent);

            var dto = Mapper.Map<Event>(newEvent);

            return CreatedAtRoute(Route.EVENT_BYID, Route.GetById(newEvent), dto);
        }

        [Route("~/" + CollectionAPI.API_PREFIX + CollectionAPI.COLLECTION_PREFIX + Route.SERIES_CONTROLLER + "/noseries/events")]
        [HttpGet]
        public Task<IHttpActionResult> EventsForNullSeries()
        {
            return EventsForSeries(null);
        }

        [Route("~/" + CollectionAPI.API_PREFIX + CollectionAPI.COLLECTION_PREFIX + Route.SERIES_CONTROLLER + "/{seriesId:int}/events")]
        [HttpGet]
        public async Task<IHttpActionResult> EventsForSeries(int? seriesId)
        {
            var allEvents = await EventStore.GetQueryableAsync();

            var eventsForSeries = from ev in allEvents
                                  where ev.SeriesID == seriesId
                                  orderby ev.Id
                                  select ev;

            return PagedAndMapped<Collection.Event, Event>(eventsForSeries);
        }
    }
}