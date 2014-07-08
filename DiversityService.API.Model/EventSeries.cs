namespace DiversityService.API.Model
{
    using System; 

    public partial class EventSeries
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public string SeriesCode { get; set; }
        public Nullable<DateTime> StartDateUTC { get; set; }
        public Nullable<DateTime> EndDateUTC { get; set; }
    }
}