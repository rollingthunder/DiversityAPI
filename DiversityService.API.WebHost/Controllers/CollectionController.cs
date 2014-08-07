using DiversityService.API.Model;
using Ninject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace DiversityService.API.Controllers
{
    public class CollectionController : ApiController
    {
        private readonly IEnumerable<InternalCollectionServer> ServerDescriptors;

        public CollectionController(IEnumerable<InternalCollectionServer> configuredServers)
        {
            ServerDescriptors = configuredServers;
        }

        public IEnumerable<CollectionServer> Get()
        {
            return ServerDescriptors;
        }        
    }
}
