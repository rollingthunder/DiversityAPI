namespace DiversityService.API.Controllers
{
    using DiversityService.API.Model;
    using DiversityService.API.Services;
    using System.Collections.Generic;
    using System.Web.Http;

    public class CollectionController : ApiController
    {
        private new readonly IConfigurationService Configuration;

        public CollectionController(IConfigurationService config)
        {
            Configuration = config;
        }

        public IEnumerable<CollectionServer> Get()
        {
            return Configuration.GetCollectionServers();
        }
    }
}