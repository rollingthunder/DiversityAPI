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
    public class SeriesController : DiversityController
    {
        private readonly IMappingService Mapper;

        private IStore<Collection.EventSeries, int> SeriesStore
        {
            get
            {
                return Request.GetCollectionContext().Series;
            }
        }

        public SeriesController(
            IMappingService mapper
            )
        {
            this.Mapper = mapper;
        }

        [Route]
        public async Task<IHttpActionResult> Get(string code = null)
        {
            var query = await SeriesStore.GetQueryableAsync();

            query = query.OrderBy(x => x.Id);

            if (!string.IsNullOrEmpty(code))
            {
                query = query
                    .Where(x => x.Code.Contains(code));
            }

            var mapped = Mapper.Project<Collection.EventSeries, EventSeries>(query);

            return Paged(mapped);
        }

        [Route]
        public async Task<IHttpActionResult> Get(int id)
        {
            var series = await SeriesStore.GetByIDAsync(id);

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
            var existing = await RedirectToExisting(SeriesStore, value.TransactionGuid);

            if (existing != null)
            {
                return existing;
            }

            var series = Mapper.Map<Collection.EventSeries>(value);

            await SeriesStore.InsertAsync(series);

            return CreatedAtRoute(Route.DEFAULT_API, Route.GetById(series), series.Id);
        }
    }
}