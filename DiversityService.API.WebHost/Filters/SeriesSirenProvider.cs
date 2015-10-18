namespace DiversityService.API.Filters
{
    using System;
    using System.Diagnostics.Contracts;
    using System.Web.Http;
    using DiversityService.API.Model;
    using WebApiContrib.Formatting.Siren.Client;

    public class SeriesSirenProvider : ISirenProvider
    {
        private HttpConfiguration configuration;

        public SeriesSirenProvider(HttpConfiguration config)
        {
            Contract.Requires<ArgumentNullException>(config != null);
            configuration = config;
        }

        public bool CanTranslate(Type type)
        {
            return type == typeof(EventSeries);
        }

        public ISirenEntity Translate(object obj)
        {
            var series = obj as EventSeries;

            var siren = new SirenEntity();

            return siren;
        }
    }
}