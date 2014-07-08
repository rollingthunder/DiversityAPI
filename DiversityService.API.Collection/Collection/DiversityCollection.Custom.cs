namespace DiversityService.Collection
{
    using DiversityService.API.WebHost;
    using System;
    using System.Globalization;

    public partial class Event : IIdentifiable
    {
        public DateTime? TimeStamp 
        {
            get
            {
                if (CollectionYear.HasValue && CollectionMonth.HasValue && CollectionDay.HasValue)
                {
                    int hours = 0;
                    int minutes = 0;
                    int seconds = 0;

                    DateTime time;
                    if(DateTime.TryParse(CollectionTime, out time)) {
                        hours = time.Hour;
                        minutes = time.Minute;
                        seconds = time.Second;
                    }


                    return new DateTime(CollectionYear.Value, CollectionMonth.Value, CollectionDay.Value, hours, minutes, seconds);
                }

                return null;
            }

            set
            {
                if (value.HasValue)
                {
                    CollectionYear = (Int16?)value.Value.Year;
                    CollectionMonth = (byte?)value.Value.Month;
                    CollectionDay = (byte?)value.Value.Day;

                    // Long Time String Invariant
                    CollectionTime = value.Value.ToString("T", CultureInfo.InvariantCulture);
                }
                else
                {
                    CollectionYear = null;
                    CollectionMonth = null;
                    CollectionDay = null;
                    CollectionTime = null;
                }
            }
        }
    }

    public partial class EventSeries : IIdentifiable
    {

    }

    public partial class Collection
    {
        public Collection(string cstring) : base(cstring)
        {
        }
    }
}
