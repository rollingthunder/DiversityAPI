using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DiversityService.API.Services
{
    public class CollectionContext : Collection.Collection
    {
        public CollectionContext(string connectionString) : base(connectionString)
        {

        }
    }
}