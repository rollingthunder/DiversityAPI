using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiversityService.API.Model
{
    public class TaxonList
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public string TaxonGroup { get; set; }
    }

    public class TaxonName
    {
        //Read-Only
        public string URI { get; set; }

        public string TaxonNameCache { get; set; }

        public string TaxonNameSinAuth { get; set; }

        public string GenusOrSupragenic { get; set; }

        public string SpeciesEpithet { get; set; }

        public string InfraspecificEpithet { get; set; }

        //Synonymy Features
        public string Synonymy { get; set; }

        public string AcceptedNameURI { get; set; }

        public string AcceptedNameCache { get; set; }
    }
}