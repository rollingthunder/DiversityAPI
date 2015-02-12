namespace DiversityService.API.Model
{
    using System;
    using System.Collections.Generic;

    public class EventSeries : IIdentifiable
    {
        public int Id { get; set; }

        public string Description { get; set; }

        public string Code { get; set; }

        public Nullable<DateTime> StartDateUTC { get; set; }

        public Nullable<DateTime> EndDateUTC { get; set; }

        public IEnumerable<Localization> Tour { get; set; }

        public override string ToString()
        {
            return string.Format("EventSeries #{0}", Id);
        }
    }

    public class EventSeriesUpload : EventSeries, IGuidIdentifiable
    {
        public Guid TransactionGuid { get; set; }
    }
}