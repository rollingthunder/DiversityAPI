namespace DiversityService.API.Services
{
    using AutoMapper;
    using DiversityService.API.Model;
    using System;
    using System.Collections.Generic;
    using System.Data.Entity.Spatial;
    using System.Diagnostics.Contracts;
    using System.Globalization;
    using System.Linq;
    using System.Text;

    public class LMapper : ITypeConverter<DbGeography, Localization>
    {
        public Localization Convert(ResolutionContext context)
        {
            throw new NotImplementedException();
        }
    }

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

        public static DbGeography ToGeography(this Localization loc)
        {
            if (loc == null)
            {
                return null;
            }

            if (double.IsNaN(loc.Latitude)
                || double.IsNaN(loc.Longitude)
                || (loc.Altitude.HasValue && double.IsNaN(loc.Altitude.Value)))
            {
                throw new ArgumentException("loc may not contain NaNs");
            }

            var cult = CultureInfo.InvariantCulture;
            var longitudeStr = loc.Longitude.ToString("R", cult);
            var latStr = loc.Latitude.ToString("R", cult);
            var altStr = loc.Altitude.HasValue ? loc.Altitude.Value.ToString("R", cult) : string.Empty;

            var ptStr = string.Format("POINT({0} {1} {2})", longitudeStr, latStr, altStr);

            return DbGeography.PointFromText(ptStr, DbGeography.DefaultCoordinateSystemId);
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

        public static DbGeography ToGeography(this IEnumerable<Localization> locs)
        {
            // The points in the LineString must be unique
            var uniqueLocs =
                (locs != null)
                ? locs.Distinct().ToList()
                : new List<Localization>();

            if (uniqueLocs.Count == 0)
            {
                return DbGeography.LineFromText("LINESTRING EMPTY", DbGeography.DefaultCoordinateSystemId);
            }
            if (uniqueLocs.Count == 1)
            {
                return uniqueLocs.First().ToGeography();
            }

            var cult = CultureInfo.InvariantCulture;

            var locStrs = from loc in uniqueLocs
                          select string.Format(cult, "{0:R} {1:R} {2:R}", loc.Longitude, loc.Latitude, loc.Altitude);

            var lineStr = string.Format("LINESTRING({0})",
                        string.Join(", ", locStrs)
                    );

            return DbGeography.LineFromText(lineStr, DbGeography.DefaultCoordinateSystemId);
        }
    }
}