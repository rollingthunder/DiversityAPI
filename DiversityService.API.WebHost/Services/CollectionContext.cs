using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DiversityService.API.Services
{
    public class CollectionContext : Collection.Collection, IContext
    {
        public CollectionContext(string connectionString)
            : base(connectionString)
        {
        }
    }
}