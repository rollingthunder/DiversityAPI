namespace DiversityService.API.Controllers
{
    using DiversityService.API.Filters;
    using DiversityService.API.Model;
    using DiversityService.API.Services;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Web.Http;

    [CollectionAPI("series")]
    public class SeriesController : DiversityController, IFieldDataController<EventSeriesBindingModel>
    {
        private IStore<Collection.EventSeries, int> Store
        {
            get
            {
                return Request.GetCollectionContext().Series;
            }
        }

        public SeriesController(
            IMappingService mapper
            )
            : base(mapper)
        {
        }

        [Route]
        public Task<IHttpActionResult> Get()
        {
            return Get(null);
        }

        [Route]
        public async Task<IHttpActionResult> Get(string code)
        {
            var allSeries = await Store.GetQueryableAsync();

            if (!string.IsNullOrEmpty(code))
            {
                allSeries = allSeries
                    .Where(x => x.Code.Contains(code));
            }

            var query = allSeries.OrderBy(x => x.Id);

            return PagedAndMapped<Collection.EventSeries, EventSeries>(query);
        }

        [Route("{id}", Name = Route.SERIES_BYID)]
        public async Task<IHttpActionResult> Get(int id)
        {
            var series = await Store.GetByIDAsync(id);

            if (series == null)
            {
                return NotFound();
            }

            var dto = Mapper.Map<EventSeries>(series);

            return Ok(dto);
        }

        [Route]
        public async Task<IHttpActionResult> Post(EventSeriesBindingModel value)
        {
            var existing = await RedirectToExisting(Store, value.TransactionGuid, Route.SERIES_BYID);

            if (existing != null)
            {
                return existing;
            }

            var series = Mapper.Map<Collection.EventSeries>(value);

            await Store.InsertAsync(series);

            var dto = Mapper.Map<EventSeries>(series);

            return CreatedAtRoute<EventSeries>(Route.SERIES_BYID, Route.GetById(series), dto);
        }
    }
}