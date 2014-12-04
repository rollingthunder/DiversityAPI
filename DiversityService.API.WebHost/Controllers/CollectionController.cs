namespace DiversityService.API.Controllers
{
    using DiversityService.API.Filters;
    using DiversityService.API.Model;
    using DiversityService.API.Services;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Http;

    [RoutePrefix(CollectionAPI.API_PREFIX + CollectionAPI.COLLECTION)]
    public class CollectionController : ApiController
    {
        private new readonly IConfigurationService Configuration;

        public CollectionController(IConfigurationService config)
        {
            Configuration = config;
        }

        [Route]
        public IEnumerable<CollectionServer> Get()
        {
            return Configuration.GetCollectionServers();
        }

        [Route(CollectionAPI.COLLECTION_TEMPLATE)]
        public CollectionServer GetById(int collection)
        {
            return Configuration
                .GetCollectionServers()
                .Where(x => x.Id == collection)
                .FirstOrDefault();
        }
    }
}