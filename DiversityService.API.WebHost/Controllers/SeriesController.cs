namespace DiversityService.API.Controllers
{
    using DiversityService.API.Model;
    using DiversityService.API.Services;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Threading.Tasks;
    using System.Web.Http;

    public class SeriesController : DiversityController
    {
        private readonly Lazy<ISeriesStore> _SeriesStore;

        private ISeriesStore SeriesStore { get { return _SeriesStore.Value; } }

        private readonly IMappingService Mapper;

        public SeriesController(
            Lazy<ISeriesStore> store,
            IMappingService mapper
            )
        {
            this._SeriesStore = store;
            this.Mapper = mapper;
        }

        public async Task<IQueryable<EventSeries>> Get(string query = null)
        {
            var seriesQuery = await SeriesStore.FindAsync();

            if (!string.IsNullOrEmpty(query))
            {
                seriesQuery = from series in seriesQuery
                              where series.Code.Contains(query)
                              select series;
            }

            return Mapper.Project<Collection.EventSeries, EventSeries>(seriesQuery);
        }

        public async Task<IHttpActionResult> Get(int id)
        {
            var series = await SeriesStore.FindAsync(id);

            if (series == null)
            {
                return NotFound();
            }

            var dto = Mapper.Map<EventSeries>(series);

            return Ok(dto);
        }

        public async Task<IHttpActionResult> Post(EventSeriesBindingModel value)
        {
            var existing = (from es in await SeriesStore.FindAsync()
                            where es.RowGUID == value.TransactionGuid
                            select es).SingleOrDefault();

            if (existing != null)
            {
                return SeeOtherAtRoute(Route.DEFAULT_API, Route.GetById(existing));
            }

            var series = Mapper.Map<Collection.EventSeries>(value);

            await SeriesStore.InsertAsync(series);

            return CreatedAtRoute(Route.DEFAULT_API, Route.GetById(series), series.Id);
        }
    }
}