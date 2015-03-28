namespace DiversityService.API.Controllers
{
    using DiversityService.API.Model;
    using System;

    public static class Route
    {
        public const string DOCUMENTATION_BASE = Relations.DOCUMENTATION_BASE;

        public const string SERIES_BYID = "SeriesById";
        public const string EVENT_BYID = "EventById";
        public const string SPECIMEN_BYID = "SpecimenById";
        public const string IDENTIFICATION_BYID = "IdentificationById";

        public const string PARAM_CONTROLLER = "controller";
        public const string PARAM_ID = "id";

        public const string PREFIX_DEFAULT_API = "api/";

        public const string ACCOUNT_CONTROLLER = "Account";
        public const string PREFIX_ACCOUNT = PREFIX_DEFAULT_API + ACCOUNT_CONTROLLER + "/";

        public const string IDENTIFICATION_CONTROLLER = "identification";

        public const string SERIES_CONTROLLER = "series";
        public const string PREFIX_SERIES = PREFIX_DEFAULT_API + SERIES_CONTROLLER + "/";

        public const string EVENT_CONTROLLER = "event";
        public const string PREFIX_EVENT = PREFIX_DEFAULT_API + EVENT_CONTROLLER + "/";

        public const string SPECIMEN_CONTROLLER = "specimen";

        public const string TAXA_CONTROLLER = "taxa";
        public const string PREFIX_TAXA = PREFIX_DEFAULT_API + TAXA_CONTROLLER + "/";

        public const string TERMS_CONTROLLER = "taxa";

        public static object GetById<T>(T entity) where T : IIdentifiable
        {
            return GetById<T>(entity.Id);
        }

        public static object GetById<T>(int id)
        {
            return new
            {
                id = id
            };
        }
    }
}