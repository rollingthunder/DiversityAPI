namespace DiversityService.API.Model
{
    using System;

    public class Event : IIdentifiable
    {
        public int Id { get; set; }

        public Nullable<int> SeriesId { get; set; }

        public string LocationDescription { get; set; }

        public string HabitatDescription { get; set; }

        public DateTime? TimeStamp { get; set; }

        public Localization Localization { get; set; }
    }

    public class EventUpload : Event, IGuidIdentifiable
    {
        public Guid TransactionGuid { get; set; }
    }
}