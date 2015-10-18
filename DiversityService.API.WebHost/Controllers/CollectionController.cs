namespace DiversityService.API.Controllers
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Http;
    using DiversityService.API.Filters;
    using DiversityService.API.Model;
    using DiversityService.API.Services;

    [RoutePrefix(CollectionAPI.ApiPrefix + CollectionAPI.Collection)]
    public class CollectionController : ApiController
    {
        private readonly IConfigurationService configuration;

        public CollectionController(IConfigurationService config)
        {
            configuration = config;
        }

        [Route]
        public IEnumerable<CollectionServer> Get()
        {
            return configuration.GetCollectionServers();
        }

        [Route(CollectionAPI.CollectionTemplate)]
        public CollectionServer GetById(int collection)
        {
            return configuration
                .GetCollectionServers()
                .Where(x => x.Id == collection)
                .FirstOrDefault();
        }
    }
}