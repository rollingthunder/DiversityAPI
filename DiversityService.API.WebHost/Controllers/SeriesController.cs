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
    [Authorize]
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
        public HttpResponseMessage Get(int id)
        {
            var Series = this.Repository.Get<Collection.EventSeries>();
            var series = Series.Find(id);

            if (series == null)
            {
                throw new HttpResponseException(new HttpResponseMessage()
                {
                    StatusCode = HttpStatusCode.NotFound,
                    ReasonPhrase = Messages.Series_NotFound
                });
            }

            var dto = new EventSeries();
            Mapper.Map(series, dto);

            return new HttpResponseMessage()
            {
                Content = new ObjectContent<EventSeries>(dto, Configuration.Formatters.JsonFormatter) 
            };
        }

        // POST api/values
        [ValidateModel]
        public async Task<IHttpActionResult> Post(EventSeries value)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var Series = Repository.Get<Collection.EventSeries>();

            var series = Mapper.Map<Collection.EventSeries>(value);

            Series.Insert(series);

            return Json(series.Id);
        }

        // PUT api/values/5
        public void Put(int id, [FromBody]EventSeries value)
        {
        }

        // DELETE api/values/5
        public void Delete(int id)
        {
        }
    }
}
