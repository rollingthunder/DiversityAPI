using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiversityService.API.Model
{
    public class Identification : IIdentifiable
    {
        public int Id { get; set; }

        public int SpecimenId { get; set; }

        public string TaxonomicGroup { get; set; }

        public string Name { get; set; }

        public string Uri { get; set; }
    }

    public class IdentificationUpload : Identification, IGuidIdentifiable
    {
        public Guid TransactionGuid { get; set; }
    }
}