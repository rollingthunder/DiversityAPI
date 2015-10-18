using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DiversityService.API.Model.Internal
{
    public class ServerTaxonList : TaxonList
    {
        public DBModule Module { get; set; }

        public int DatabaseId { get; set; }
    }
}