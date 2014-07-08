using DiversityService.API.Model;
using Ninject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace DiversityService.API.WebHost.Controllers
{
    public class CollectionController : ApiController
    {
        private readonly IEnumerable<CollectionServer> ServerDescriptors;

        public CollectionController(IEnumerable<CollectionServer> configuredServers)
        {
            ServerDescriptors = configuredServers;
        }

        public IEnumerable<CollectionServer> Get()
        {
            return ServerDescriptors;
        }        
    }
}
