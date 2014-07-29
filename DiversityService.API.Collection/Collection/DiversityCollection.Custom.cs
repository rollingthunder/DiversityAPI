namespace DiversityService.Collection
{
    using DiversityService.API.WebHost;
    using System;
    using System.Linq;
    using System.Collections;
    using System.Globalization;
    using System.Data.Entity.Infrastructure;
    using System.Data.Entity.Core.Objects;
    using System.Data.Entity;

    public partial class Event : IIdentifiable, IGuidIdentifiable
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

    public partial class EventSeries : IIdentifiable, IGuidIdentifiable
    {

    }

    public partial class Specimen : IIdentifiable
    {
        public const string DATECATEGORY_COLLECTIONDATE = "collection date";

        public DateTime? CollectionDate
        {
            get
            {
                if (AccessionYear.HasValue && AccessionMonth.HasValue && AccessionDay.HasValue)
                {
                    return new DateTime(AccessionYear.Value, AccessionMonth.Value, AccessionDay.Value);
                }

                return null;
            }

            set
            {
                if (value.HasValue)
                {
                    AccessionYear = (Int16?)value.Value.Year;
                    AccessionMonth = (byte?)value.Value.Month;
                    AccessionDay = (byte?)value.Value.Day;
                    AccessionDate = value.Value.Date;
                    AccessionDateCategory = DATECATEGORY_COLLECTIONDATE;
                }
                else
                {
                    AccessionYear = null;
                    AccessionMonth = null;
                    AccessionDay = null;
                    AccessionDate = null;
                    AccessionDateCategory = null;
                }
            }
        }
    }

    public partial class IdentificationUnit : IIdentifiable
    {

    }

    public partial class Collection
    {
        public Collection(string cstring) : base(cstring)
        {
            ((IObjectContextAdapter)this).ObjectContext.ObjectMaterialized +=
                (sender, e) => ForceUTC(e.Entity);
            ((IObjectContextAdapter)this).ObjectContext.SavingChanges +=
                (sender, e) => ForceLocal(sender);
        }


        public static void ForceUTC(object entity)
        {
            if (entity == null)
                return;

            var properties = entity.GetType().GetProperties()
                .Where(x => x.PropertyType == typeof(DateTime) || x.PropertyType == typeof(DateTime?));

            foreach (var property in properties)
            {
                var dt = property.PropertyType == typeof(DateTime?)
                    ? (DateTime?)property.GetValue(entity)
                    : (DateTime)property.GetValue(entity);

                if (dt == null)
                    continue;

                // Assume Local Times in DB 
                // Convert to UTC

                property.SetValue(entity, dt.Value.ToUniversalTime());
            }
        }

        public static void ForceLocal(object sender)
        {
            // Ensure that we are passed an ObjectContext
            ObjectContext context = sender as ObjectContext;
            if (context != null)
            {
                foreach (ObjectStateEntry entry in
                    context.ObjectStateManager.GetObjectStateEntries(
                    EntityState.Added | EntityState.Modified))
                {
                    var entity = entry.Entity;

                    if (entity == null)
                        continue;

                    var properties = entity.GetType().GetProperties()
                        .Where(x => x.PropertyType == typeof(DateTime) || x.PropertyType == typeof(DateTime?));

                    foreach (var property in properties)
                    {
                        var dt = property.PropertyType == typeof(DateTime?)
                            ? (DateTime?)property.GetValue(entity)
                            : (DateTime)property.GetValue(entity);

                        if (dt == null)
                            continue;

                        // Store only local time into DB (mainly for backward compat)
                        property.SetValue(entity, dt.Value.ToLocalTime());
                    }
                }
            }
        }
    }
}
