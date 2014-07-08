using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DiversityService.API.Model
{
    public partial class CollectionServer
    {
        [JsonIgnore]
        public string Address { get; set; }

        [JsonIgnore]
        public int Port { get; set; }
        [JsonIgnore]
        public string Catalog { get; set; }
    }
}