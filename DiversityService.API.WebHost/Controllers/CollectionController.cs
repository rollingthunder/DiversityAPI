namespace DiversityService.API.Controllers
{
    using DiversityService.API.Model;
    using DiversityService.API.Services;
    using Ninject;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Web.Http;

    public class CollectionController : ApiController
    {
        private readonly IConfigurationService Configuration;

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