using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DiversityService.API.Model
{
    public class InternalCollectionServer : CollectionServer
    {
        [JsonIgnore]
        public string Address { get; set; }

        [JsonIgnore]
        public int Port { get; set; }

        [JsonIgnore]
        public string Catalog { get; set; }
    }

    public class CollectionServerLogin : InternalCollectionServer
    {
        [JsonIgnore]
        public string User { get; set; }

        [JsonIgnore]
        public string Password { get; set; }
    }
}