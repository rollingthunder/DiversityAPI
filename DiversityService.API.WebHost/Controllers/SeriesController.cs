using DiversityService.API.Model;
using DiversityService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using AutoMapper.QueryableExtensions;
using AutoMapper;
using DiversityService.API.Resources;
using DiversityService.API.WebHost.Filters;
using System.Threading.Tasks;

namespace DiversityService.API.WebHost.Controllers
{
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
        
        [Queryable(PageSize=20)]        
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
            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var Series = Repository.Get<Collection.EventSeries>();            

            var series = Mapper.Map<Collection.EventSeries>(value);

            Series.Insert(series);
            Series.Transaction.Save();

            return Json(series.Id);
        }

        // PUT api/values/5
        public void Put(int id, [FromBody]EventSeries value)
        {
        }
    }
}
