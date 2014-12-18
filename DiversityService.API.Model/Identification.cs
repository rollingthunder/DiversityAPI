using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiversityService.API.Model
{
    public class IdentificationCommon
    {
        public string TaxonomicGroup { get; set; }
        public string Name { get; set; }
        public string Uri { get; set; }
    }

    public class Identification
    {
        public int Id { get; set; }        
    }

    public class IdentificationUpload: IdentificationCommon, IGuidIdentifiable
    {
        public Guid TransactionGuid { get; set; }
    }
}
