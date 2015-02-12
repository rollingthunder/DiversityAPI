namespace DiversityService.API.Model
{
    using NodaTime;
    using System;

    public class Specimen : IIdentifiable
    {
        public int Id { get; set; }

        public int? EventId { get; set; }

        public string AccessionNumber { get; set; }

        public LocalDate? CollectionDate { get; set; }
    }

    public class SpecimenUpload : Specimen, IGuidIdentifiable
    {
        public Guid TransactionGuid { get; set; }
    }
}