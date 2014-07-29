﻿namespace DiversityService.API.WebHost.Controllers
{
    using AutoMapper;
    using AutoMapper.QueryableExtensions;
    using DiversityService.API.Model;
    using DiversityService.API.WebHost.Filters;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Threading.Tasks;
    using System.Web.Http;

    public class SeriesController : ApiController
    {
        private readonly IRepositoryFactory Repository;
        private readonly IMappingEngine Mapper;


        public SeriesController(
            IRepositoryFactory repo,
            AutoMapper.IMappingEngine mapper
            )
        {
            this.Repository = repo;
            this.Mapper = mapper;
        }

        [Queryable(PageSize = 20)]
        public IQueryable<EventSeries> Get()
        {
            var Series = this.Repository.Get<Collection.EventSeries>();

            return Series.All
                .Project(Mapper)
                .To<EventSeries>();
        }

        // GET api/series/5
        public async Task<IHttpActionResult> Get(int id)
        {
            var Series = this.Repository.Get<Collection.EventSeries>();
            var series = await Series.Find(id);

            if (series == null)
            {
                return NotFound();
            }

            var dto = Mapper.Map<EventSeries>(series);

            return Json(dto);
        }

        // POST api/values
        [ValidateModel]
        public async Task<IHttpActionResult> Post(EventSeriesBindingModel value)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var Series = Repository.Get<Collection.EventSeries>();

            var existing = (from es in Series.All
                            where es.RowGUID == value.TransactionGuid
                            select es).SingleOrDefault();

            if(existing != null)
            {
                return this.SeeOtherAtRoute(Route.DEFAULT_API, Route.GetById(existing), existing.Id);
            }


            var series = Mapper.Map<Collection.EventSeries>(value);

            Series.Insert(series);
            Series.Transaction.Save();

            return CreatedAtRoute(Route.DEFAULT_API, Route.GetById(series), series.Id);
        }

        // PUT api/values/5
        public void Put(int id, [FromBody]EventSeries value)
        {
        }
    }
}
