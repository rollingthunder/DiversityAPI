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
        private readonly IMappingService Mapper;

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
        {
            this.Mapper = mapper;
        }

        [Route]
        public Task<IHttpActionResult> Get()
        {
            return Get(null);
        }

        [Route]
        public async Task<IHttpActionResult> Get(string code)
        {
            var query = await Store.GetQueryableAsync();

            query = query.OrderBy(x => x.Id);

            if (!string.IsNullOrEmpty(code))
            {
                query = query
                    .Where(x => x.Code.Contains(code));
            }

            var mapped = Mapper.Project<Collection.EventSeries, EventSeries>(query);

            return Paged(mapped);
        }

        [Route("{id}")]
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
            var existing = await RedirectToExisting(Store, value.TransactionGuid);

            if (existing != null)
            {
                return existing;
            }

            var series = Mapper.Map<Collection.EventSeries>(value);

            await Store.InsertAsync(series);

            return CreatedAtRoute(Route.DEFAULT_API, Route.GetById(series), series.Id);
        }
    }
}