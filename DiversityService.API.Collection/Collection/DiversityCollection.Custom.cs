namespace DiversityService.Collection
{
    using DiversityService.API.Model;
    using NodaTime;
    using System;
    using System.Collections;
    using System.Data.Entity;
    using System.Data.Entity.Core.Objects;
    using System.Data.Entity.Infrastructure;
    using System.Globalization;
    using System.Linq;

    public partial class Event : IIdentifiable, IGuidIdentifiable
    {
        internal const string COLLECTIONDATECATEGORY_ACTUAL = "actual";

        internal static void LoadTimeStamp(Event ev)
        {
            if (ev.TimeStamp.HasValue)
            {
                // TimeStamp only holds the date
                // Merge the Time into the Date
                if (!string.IsNullOrWhiteSpace(ev.CollectionTime))
                {
                    TimeSpan time;

                    if (TimeSpan.TryParse(ev.CollectionTime, out time))
                    {
                        // If there is a time given, we have a complete timestamp
                        // So we convert it to UTC
                        ev.TimeStamp = EntityHelper.ForceUTC(ev.TimeStamp.Value.Date + time);
                    }
                    else
                    {
                        // If there is no time, all we have is a date
                        // So it would not make sense to convert to UTC
                    }
                }
            }
        }

        internal static void StoreTimeStamp(Event ev)
        {
            if (ev.TimeStamp.HasValue)
            {
                var timeStamp = ev.TimeStamp.Value;

                // Does the TimeStamp hold only date info or a time as well?
                if (timeStamp.TimeOfDay != TimeSpan.MinValue)
                {
                    // Presumably, we have a complete TimeStamp
                    // So we need to convert to Local Time
                    var localTimeStamp = EntityHelper.ForceLocal(timeStamp).Value;

                    // The TimeStamp field in the DB stores _only_ the date
                    timeStamp = localTimeStamp.Date;

                    var timeOfDay = localTimeStamp.TimeOfDay;
                    // Discard fractional seconds, DB stores only hh:mm:ss
                    var secondTimeOfDay = new TimeSpan(timeOfDay.Hours, timeOfDay.Minutes, timeOfDay.Seconds);
                    // Convert to invariant string ([-][d.]hh:mm:ss[.fffffff])
                    ev.CollectionTime = secondTimeOfDay.ToString("c");
                }
                else
                {
                    // No Time, only Date information
                    ev.CollectionTime = string.Empty;
                }

                // Update Y/M/D Columns from the current TimeStamp
                ev.CollectionYear = (short)timeStamp.Year;
                ev.CollectionMonth = (byte)timeStamp.Month;
                ev.CollectionDay = (byte)timeStamp.Day;

                // If the category has not been determined yet, set it.
                if (string.IsNullOrWhiteSpace(ev.CollectionDateCategory))
                {
                    ev.CollectionDateCategory = COLLECTIONDATECATEGORY_ACTUAL;
                }
            }
            else
            {
                ev.CollectionYear = null;
                ev.CollectionMonth = null;
                ev.CollectionDay = null;
                ev.CollectionTime = null;
                ev.CollectionDateCategory = null;
            }
        }

        internal static void OnMaterialized(Event ev)
        {
            LoadTimeStamp(ev);
        }

        internal static void BeforeSave(Event ev)
        {
            StoreTimeStamp(ev);
        }
    }

    public partial class EventSeries : IIdentifiable, IGuidIdentifiable
    {
        internal static void OnMaterialized(EventSeries es)
        {
            es.StartDateUTC = EntityHelper.ForceUTC(es.StartDateUTC);
            es.EndDateUTC = EntityHelper.ForceUTC(es.EndDateUTC);
        }

        internal static void BeforeSave(EventSeries es)
        {
            es.StartDateUTC = EntityHelper.ForceLocal(es.StartDateUTC);
            es.EndDateUTC = EntityHelper.ForceLocal(es.EndDateUTC);
        }
    }

    public partial class Specimen : IIdentifiable, IGuidIdentifiable
    {
        public const string DATECATEGORY_COLLECTIONDATE = "collection date";

        public LocalDate? GetCollectionDate()
        {
            if (AccessionYear.HasValue && AccessionMonth.HasValue && AccessionDay.HasValue)
            {
                return new LocalDate(AccessionYear.Value, AccessionMonth.Value, AccessionDay.Value);
            }

            return null;
        }

        public void SetCollectionDate(LocalDate? value)
        {
            if (value.HasValue)
            {
                AccessionYear = (Int16?)value.Value.Year;
                AccessionMonth = (byte?)value.Value.Month;
                AccessionDay = (byte?)value.Value.Day;
                AccessionDate = value.Value.AtMidnight().ToDateTimeUnspecified();
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

    public partial class IdentificationUnit : IIdentifiable, IGuidIdentifiable, ICompositeIdentifiable<IdentificationUnitKey>
    {
        public IdentificationUnitKey CompositeKey()
        {
            return new IdentificationUnitKey()
            {
                IdentificationUnitId = Id,
                SpecimenId = SpecimenId
            };
        }
    }

    public struct IdentificationUnitKey : ICompositeKey, IEquatable<IdentificationUnitKey>
    {
        public int SpecimenId;
        public int IdentificationUnitId;

        public object[] Values()
        {
            return new object[]{
                SpecimenId,
                IdentificationUnitId
            };
        }

        public override bool Equals(object obj)
        {
            if (obj is IdentificationUnitKey)
            {
                return this.Equals((IdentificationUnitKey)obj);
            }

            return false;
        }

        public override int GetHashCode()
        {
            return ((this.SpecimenId * 251) + this.IdentificationUnitId);
        }

        public bool Equals(IdentificationUnitKey other)
        {
            return SpecimenId == other.SpecimenId &&
                   IdentificationUnitId == other.IdentificationUnitId;
        }
    }

    public partial class Identification : ICompositeIdentifiable<IdentificationKey>
    {
        public const string DATECATEGORY_ACTUAL = "actual";

        public Identification()
        {
            RowGUID = Guid.NewGuid();
        }

        public LocalDate? GetCollectionDate()
        {
            if (IdentificationYear.HasValue && IdentificationMonth.HasValue && IdentificationDay.HasValue)
            {
                return new LocalDate(IdentificationYear.Value, IdentificationMonth.Value, IdentificationDay.Value);
            }

            return null;
        }

        public void SetCollectionDate(LocalDate? value)
        {
            if (value.HasValue)
            {
                IdentificationYear = (Int16?)value.Value.Year;
                IdentificationMonth = (byte?)value.Value.Month;
                IdentificationDay = (byte?)value.Value.Day;
                IdentificationDate = value.Value.AtMidnight().ToDateTimeUnspecified();
                IdentificationDateCategory = DATECATEGORY_ACTUAL;
            }
            else
            {
                IdentificationYear = null;
                IdentificationMonth = null;
                IdentificationDay = null;
                IdentificationDate = null;
                IdentificationDateCategory = null;
            }
        }

        public IdentificationKey CompositeKey()
        {
            return new IdentificationKey()
            {
                SpecimenId = SpecimenID,
                IdentificationUnitId = IdentificationUnitID,
                IdentificationId = Id
            };
        }
    }

    public struct IdentificationKey : ICompositeKey
    {
        public int SpecimenId;
        public int IdentificationUnitId;
        public int IdentificationId;

        public object[] Values()
        {
            return new object[]{
                SpecimenId,
                IdentificationUnitId,
                IdentificationId
            };
        }
    }

    public partial class IdentificationUnitGeoAnalysis : ICompositeIdentifiable<IdentificationGeoKey>
    {
        public IdentificationUnitGeoAnalysis()
        {
            RowGUID = Guid.NewGuid();
        }

        public IdentificationGeoKey CompositeKey()
        {
            return new IdentificationGeoKey()
            {
                SpecimenId = SpecimenId,
                IdentificationUnitId = IdentificationUnitId,
                AnalysisDate = AnalysisDate
            };
        }
    }

    public struct IdentificationGeoKey : ICompositeKey
    {
        public int SpecimenId;
        public int IdentificationUnitId;
        public DateTime AnalysisDate;

        public object[] Values()
        {
            return new object[]{
                SpecimenId,
                IdentificationUnitId,
                AnalysisDate
            };
        }
    }

    public static class EntityHelper
    {
        public static DateTime? ForceUTC(DateTime? time)
        {
            if (time.HasValue)
            {
                time = time.Value.ToUniversalTime();
            }

            return time;
        }

        public static DateTime? ForceLocal(DateTime? time)
        {
            if (time.HasValue)
            {
                time = time.Value.ToLocalTime();
            }

            return time;
        }

        internal static void SavingChanges(object sender, EventArgs e)
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

                    if (entity is EventSeries)
                    {
                        EventSeries.BeforeSave(entity as EventSeries);
                    }
                    else if (entity is Event)
                    {
                        Event.BeforeSave(entity as Event);
                    }
                }
            }
        }

        internal static void ObjectMaterialized(object sender, ObjectMaterializedEventArgs e)
        {
            var entity = e.Entity;

            if (entity == null)
            {
                return;
            }

            if (entity is EventSeries)
            {
                EventSeries.OnMaterialized(entity as EventSeries);
            }
            else if (entity is Event)
            {
                Event.OnMaterialized(entity as Event);
            }
        }
    }

    public partial class Collection
    {
        public int? ProjectId { get; set; }

        public Collection(string cstring)
            : base(cstring)
        {
            ((IObjectContextAdapter)this).ObjectContext.ObjectMaterialized += EntityHelper.ObjectMaterialized;

            ((IObjectContextAdapter)this).ObjectContext.SavingChanges += EntityHelper.SavingChanges;
        }
    }

    public class TaxonListForProject
    {
        public int Id { get; set; }

        public int ProjectId { get; set; }

        public string Name { get; set; }
    }
}