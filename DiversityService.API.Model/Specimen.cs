namespace DiversityService.API.Model
{
    using System;

    public class SpecimenCommon
    {
        public string AccessionNumber { get; set; }
        public DateTime? CollectionDate { get; set; }
    }

    public class Specimen : SpecimenCommon
    {
        public int Id { get; set; }
    }

    public class SpecimenUpload : SpecimenCommon, ITransactedModel
    {
        public Guid TransactionGuid { get; set; }
    }
}
