namespace DiversityService.API.Model
{
    using NodaTime;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class Identification : IIdentifiable
    {
        public int Id { get; set; }

        public int SpecimenId { get; set; }

        public string TaxonomicGroup { get; set; }

        public string Name { get; set; }

        public string Uri { get; set; }

        public Localization Localization { get; set; }

        public int? RelatedId { get; set; }

        public string RelationType { get; set; }

        public LocalDate Date { get; set; }
    }

    public class IdentificationUpload : Identification, IGuidIdentifiable
    {
        public Guid TransactionGuid { get; set; }
    }
}