namespace DiversityService.API.Model
{
    using System;

    public class Localization : IEquatable<Localization>
    {
        public double Longitude { get; set; }

        public double Latitude { get; set; }

        public double? Altitude { get; set; }

        public Localization()
        {
        }

        public Localization(double lon, double lat, double? alt = null)
        {
            Longitude = lon;
            Latitude = lat;
            Altitude = alt;
        }

        public bool Equals(Localization other)
        {
            if (other == null)
            {
                return false;
            }

            return this.Longitude == other.Longitude
                && this.Latitude == other.Latitude
                && this.Altitude == other.Altitude;
        }

        public override bool Equals(object obj)
        {
            if (obj is Localization)
            {
                return this.Equals(obj as Localization);
            }

            return false;
        }

        public override int GetHashCode()
        {
            return ((this.Longitude.GetHashCode() * 251) + this.Latitude.GetHashCode()) * 251 + (this.Altitude ?? 0).GetHashCode();
        }
    }
}