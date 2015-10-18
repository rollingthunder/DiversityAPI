namespace DiversityService.API.Filters
{
    using DiversityService.API.Model;
    using System;
    using System.Diagnostics.Contracts;
    using System.Web.Http;
    using WebApiContrib.Formatting.Siren.Client;

    public class SeriesSirenProvider : ISirenProvider
    {
        private HttpConfiguration Configuration;

        public SeriesSirenProvider(HttpConfiguration config)
        {
            Contract.Requires<ArgumentNullException>(config != null);
            Configuration = config;
        }

        public bool CanTranslate(Type type)
        {
            return type == typeof(EventSeries);
        }

        public ISirenEntity Translate(object obj)
        {
            Contract.Requires<ArgumentException>(obj is EventSeries);

            var series = obj as EventSeries;

            var siren = new SirenEntity();

            return siren;
        }
    }
}