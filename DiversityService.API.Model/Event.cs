namespace DiversityService.API.Model
{
    using System;

    public class EventCommon
    {
        public Nullable<int> SeriesId { get; set; }
        public string LocationDescription { get; set; }
        public string HabitatDescription { get; set; }
        public DateTime TimeStamp { get; set; }
        public Localization Localization { get; set; }
    }

    public class Event : EventCommon
    {
        public int Id { get; set; }
    }

    public class EventUpload : EventCommon, ITransactedModel
    {
        public Guid TransactionGuid { get; set; }
    }
}
