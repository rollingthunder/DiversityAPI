
namespace DiversityService.API.Controllers
{
    using DiversityService.API.Model;
    using System;

    public static class Route
    {
        // Default Route
        public const string DEFAULT_API = "DefaultApi";
        public const string PARAM_CONTROLLER = "controller";
        public const string PARAM_ID = "id";



        public const string PREFIX_DEFAULT_API = "api/";

        public const string SERIES_CONTROLLER = "series";
        public const string PREFIX_SERIES = PREFIX_DEFAULT_API + SERIES_CONTROLLER + "/";

        public const string EVENT_CONTROLLER = "event";
        public const string PREFIX_EVENT = PREFIX_DEFAULT_API + EVENT_CONTROLLER + "/";

        public static object GetById<T>(T entity) where T : IIdentifiable
        {
            return GetById<T>(entity.Id);
        }

        public static object GetById<T>(int id)
        {
            var controller = string.Empty;

            if (typeof(T) == typeof(EventSeries) || typeof(T) == typeof(Collection.EventSeries))
            {
                controller = SERIES_CONTROLLER;
            }
            else if (typeof(T) == typeof(Event) || typeof(T) == typeof(Collection.Event))
            {
                controller = EVENT_CONTROLLER;
            }

            if(controller == string.Empty) {
                throw new InvalidOperationException("Cannot determine Route, unknown Entity Type");
            }

            return new
            {
                controller = controller,
                id = id
            };
        }
    }
}