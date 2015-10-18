namespace DiversityService.API.Controllers
{
    using System.Linq;
    using System.Threading.Tasks;
    using System.Web.Http;
    using AutoMapper;
    using DiversityService.API.Filters;
    using DiversityService.API.Model;
    using DiversityService.API.Services;
    using Collection = DiversityService.DB.Collection;

    [CollectionAPI(Route.EventController)]
    public class EventController : DiversityController, IFieldDataController<EventBindingModel>
    {
        public EventController(
               IMappingEngine mapper)
               : base(mapper)
        {
        }

        private IStore<Collection.Event, int> EventStore
        {
            get
            {
                return Request.GetCollectionContext().Events;
            }
        }

        [Route("~/" + CollectionAPI.ApiPrefix + CollectionAPI.CollectionPrefix + Route.SeriesController + "/noseries/events")]
        [HttpGet]
        public Task<IHttpActionResult> EventsForNullSeries()
        {
            return EventsForSeries(null);
        }

        [Route("~/" + CollectionAPI.ApiPrefix + CollectionAPI.CollectionPrefix + Route.SeriesController + "/{seriesId:int}/events")]
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

        [Route]
        public async Task<IHttpActionResult> Get()
        {
            var allEvents = await EventStore.GetQueryableAsync();

            var query = allEvents.OrderBy(x => x.Id);

            return PagedAndMapped<Collection.Event, Event>(query);
        }

        [Route("{id}", Name = Route.EventById)]
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
            var existing = await RedirectToExisting(EventStore, value.TransactionGuid, Route.EventById);

            if (existing != null)
            {
                return existing;
            }

            var newEvent = Mapper.Map<Collection.Event>(value);

            await EventStore.InsertAsync(newEvent);

            var dto = Mapper.Map<Event>(newEvent);

            return CreatedAtRoute(Route.EventById, Route.GetById(newEvent), dto);
        }
    }
}