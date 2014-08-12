namespace DiversityService.API.Controllers
{
    using DiversityService.API.Model;
    using DiversityService.API.Services;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Threading.Tasks;
    using System.Web.Http;

    public class SeriesController : DiversityController
    {
        private readonly ISeriesStore SeriesStore;
        private readonly IMappingService Mapper;

        public SeriesController(
            ISeriesStore store,
            IMappingService mapper
            )
        {
            this.SeriesStore = store;
            this.Mapper = mapper;
        }

        public async Task<IQueryable<EventSeries>> Get()
        {
            var allSeries = await SeriesStore.FindAsync();
            return Mapper.Project<Collection.EventSeries, EventSeries>(allSeries);
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
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var existing = (from es in await SeriesStore.FindAsync()
                            where es.RowGUID == value.TransactionGuid
                            select es).SingleOrDefault();

            if(existing != null)
            {
                return SeeOtherAtRoute(Route.DEFAULT_API, Route.GetById(existing));
            }

            var series = Mapper.Map<Collection.EventSeries>(value);

            await SeriesStore.InsertAsync(series);

            return CreatedAtRoute(Route.DEFAULT_API, Route.GetById(series), series.Id);
        }

        //public void Put(int id, [FromBody]EventSeries value)
        //{
        //}
    }
}
