namespace DiversityService.API
{
    using System;

    public static class Parameters
    {
        private const string BASE_URL = Relations.DOCUMENTATION_BASE + "params/";

        public static readonly Uri SERIES_ID = new Uri(BASE_URL + "trip_id", UriKind.Absolute);
    }
}