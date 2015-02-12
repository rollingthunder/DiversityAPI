namespace DiversityService.API.Services
{
    using DiversityService.API.Model;
    using System;
    using System.Collections.Generic;
    using System.Data.Entity.Spatial;
    using System.Linq;

    public static class LocalizationMapper
    {
        public static Localization ToLocalization(this DbGeography geo)
        {
            if (geo == null)
            {
                return null;
            }

            if (geo.ElementCount != 1)
            {
                throw new ArgumentException("geo must only contain a single point");
            }

            if (!geo.Longitude.HasValue || !geo.Latitude.HasValue)
            {
                throw new ArgumentException("geo must have a longitude and latitude");
            }

            return new Localization()
            {
                Altitude = geo.Elevation,
                Latitude = geo.Latitude.Value,
                Longitude = geo.Longitude.Value
            };
        }

        public static Localization ToGeography(this DbGeography geo)
        {
            if (geo == null)
            {
                return null;
            }

            if (geo.ElementCount != 1)
            {
                throw new ArgumentException("geo must only contain a single point");
            }

            if (!geo.Longitude.HasValue || !geo.Latitude.HasValue)
            {
                throw new ArgumentException("geo must have a longitude and latitude");
            }

            return new Localization()
            {
                Altitude = geo.Elevation,
                Latitude = geo.Latitude.Value,
                Longitude = geo.Longitude.Value
            };
        }

        public static IEnumerable<Localization> ToTour(this DbGeography geo)
        {
            if (geo != null)
            {
                var count = geo.PointCount;
                if (count.HasValue)
                {
                    if (count.Value > 0) // Geography Collection
                    {
                        var tour = new List<Localization>(count.Value);

                        // 1-based indexing!
                        for (var i = 1; i <= count.Value; ++i)
                        {
                            tour.Add(geo.PointAt(i).ToLocalization());
                        }

                        return tour;
                    }
                }
                else // Single Geography
                {
                    return Enumerable.Repeat(geo.ToLocalization(), 1);
                }
            }

            return Enumerable.Empty<Localization>();
        }
    }
}