namespace DiversityService.API.Model
{
    using System; 

    public class EventSeriesCommon
    {
        public string Description { get; set; }
        public string Code { get; set; }
        public Nullable<DateTime> StartDateUTC { get; set; }
        public Nullable<DateTime> EndDateUTC { get; set; }
    }

    public class EventSeries : EventSeriesCommon
    {
        public int Id { get; set; }
    }

    public class EventSeriesUpload : EventSeriesCommon, ITransactedModel
    {
        public Guid TransactionGuid { get; set; }
    }
}