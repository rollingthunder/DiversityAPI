namespace DiversityService.API.WebHost.Controllers
{
    using AutoMapper;
    using AutoMapper.QueryableExtensions;
    using DiversityService.API.Model;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Web.Http;

    public class EventController : ApiController
    {
        private readonly IRepositoryFactory Repository;
        private readonly IMappingEngine Mapper;

        public EventController(
            IRepositoryFactory repo,
            AutoMapper.IMappingEngine mapper
            )
        {
            this.Repository = repo;
            this.Mapper = mapper;
        }

        // GET: api/Event
        [Queryable(PageSize = 20)]  
        public IQueryable<Event> Get()
        {
            var Events = this.Repository.Get<Collection.Event>();

            return Events.All
                .Project(Mapper)
                .To<Event>();
        }

        // GET: api/Event/5
        public Event Get(int id)
        {
            return null;
        }

        // POST: api/Event
        public void Post([FromBody]EventBindingModel value)
        {
        }

        // PUT: api/Event/5
        public void Put(int id, [FromBody]Event value)
        {
        }
    }
}
