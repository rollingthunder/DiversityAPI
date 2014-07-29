
namespace DiversityService.API.WebHost.Controllers
{
    using DiversityService.API.Model;
    using System;

    public static class Route
    {
        public const string DEFAULT_API = "DefaultApi";

        public const string SERIES_CONTROLLER = "series";
        public const string EVENT_CONTROLLER = "event";

        public static object GetById<T>(T entity) where T : IIdentifiable
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
                id = entity.Id
            };
        }
    }
}