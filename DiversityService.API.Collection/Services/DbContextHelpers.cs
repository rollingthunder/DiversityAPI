using DiversityService.API.Model.Internal;
using System;
using System.Configuration;

namespace DiversityService.DB.Services
{
    public static class DbContextHelpers
    {
        public static Module.Module CreateModule(CollectionServerLogin server)
        {
            return new Module.Module(GetConnectionString(server, DBModuleType.Unknown));
        }

        public static Collection.DiversityCollection CreateCollection(CollectionServerLogin server)
        {
            return new Collection.DiversityCollection(GetConnectionString(server, DBModuleType.Collection));
        }

        public static TaxonNames.TaxonNames CreateTaxonNames(CollectionServerLogin server)
        {
            return new TaxonNames.TaxonNames(GetConnectionString(server, DBModuleType.TaxonNames));
        }

        internal static string GetConnectionString(CollectionServerLogin server, DBModuleType contextType)
        {
            string key = contextType.ToString();
            var connectionSettings = ConfigurationManager.ConnectionStrings[key];

            if (connectionSettings == null)
                throw new ArgumentException("No Connection String Template for Module Type", "contextType");

            return string.Format(
                connectionSettings.ConnectionString,
                server.Address,
                server.Port,
                server.Catalog,
                server.User,
                server.Password
            );
        }
    }
}