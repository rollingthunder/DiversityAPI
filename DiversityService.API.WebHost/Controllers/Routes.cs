namespace DiversityService.API.Controllers
{
    using DiversityService.API.Model;

    public static class Route
    {
        public const string AccountController = "Account";
        public const string DocumentationBase = Relations.DocumentationBase;

        public const string EventById = "EventById";
        public const string EventController = "event";
        public const string IdentificationById = "IdentificationById";
        public const string IdentificationController = "identification";
        public const string ParamController = "controller";
        public const string ParamId = "id";
        public const string PrefixAccount = PrefixDefaultApi + AccountController + "/";
        public const string PrefixDefaultApi = "api/";
        public const string SeriesById = "SeriesById";
        public const string SeriesController = "series";
        public const string SpecimenById = "SpecimenById";
        public const string SpecimenController = "specimen";

        public const string TaxaController = "taxa";

        public const string TermsController = "terms";

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