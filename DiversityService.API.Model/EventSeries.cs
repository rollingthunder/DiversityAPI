namespace DiversityService.API.Model
{
    using System;

    public class EventSeriesCommon
    {
        public Nullable<int> Id { get; set; }

        public string Description { get; set; }

        public string Code { get; set; }

        public Nullable<DateTime> StartDateUTC { get; set; }

        public Nullable<DateTime> EndDateUTC { get; set; }

        public override string ToString()
        {
            return string.Format("EventSeries #{0}", Id);
        }
    }

    public class EventSeries : EventSeriesCommon { }

    public class EventSeriesUpload : EventSeriesCommon, ITransactedModel
    {
        public Guid TransactionGuid { get; set; }
    }
}