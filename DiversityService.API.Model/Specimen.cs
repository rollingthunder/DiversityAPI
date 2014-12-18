namespace DiversityService.API.Model
{
    using System;

    public class Specimen : IIdentifiable
    {
        public int Id { get; set; }

        public int? EventId { get; set; }

        public string AccessionNumber { get; set; }

        public DateTime? CollectionDate { get; set; }
    }

    public class SpecimenUpload : Specimen, IGuidIdentifiable
    {
        public Guid TransactionGuid { get; set; }
    }
}