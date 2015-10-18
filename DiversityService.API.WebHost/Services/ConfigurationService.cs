namespace DiversityService.API.Services
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Linq;
    using AutoMapper;
    using DiversityService.API.Model.Internal;
    using DiversityService.API.WebHost;
    using Splat;

    public class ConfigurationService : IConfigurationService, IEnableLogger
    {
        private readonly IMappingEngine mapper;
        private readonly Lazy<CollectionServerConfigurationSection> section;
        private IDictionary<string, IEnumerable<CollectionServerLogin>> publicServers;
        private IEnumerable<InternalCollectionServer> servers;

        public ConfigurationService(
            IMappingEngine mapper)
        {
            this.mapper = mapper;

            section = new Lazy<CollectionServerConfigurationSection>(() =>
            {
                return (CollectionServerConfigurationSection)ConfigurationManager.GetSection("collectionServers");
            });
        }

        #region IConfigurationService

        public IEnumerable<InternalCollectionServer> GetCollectionServers()
        {
            LoadConfigurationIfNecessary();

            return servers;
        }

        public CollectionServerLogin GetPublicLogin(string kind)
        {
            LoadConfigurationIfNecessary();

            IEnumerable<CollectionServerLogin> logins;
            if (publicServers.TryGetValue(kind, out logins))
            {
                return logins.First();
            }

            return null;
        }

        #endregion IConfigurationService

        private IEnumerable<InternalCollectionServer> LoadCollectionServers()
        {
            var collectionServers = section.Value.Servers.Cast<CollectionServerElement>()
                .Select(mapper.Map<InternalCollectionServer>)
                .ToList();

            // Throws if there are duplicate Ids 
            collectionServers.ToDictionary(x => x.Id);

            return collectionServers;
        }

        private void LoadConfigurationIfNecessary()
        {
            if (section.IsValueCreated)
            {
                return;
            }

            publicServers = LoadPublicServers();
            servers = LoadCollectionServers();
        }

        private IDictionary<string, IEnumerable<CollectionServerLogin>> LoadPublicServers()
        {
            var res = new Dictionary<string, IEnumerable<CollectionServerLogin>>();

            foreach (var server in section.Value.PublicServers.Cast<ServerLoginCatalogElement>())
            {
                var login = mapper.Map<CollectionServerLogin>(server);

                if (!res.ContainsKey(login.Kind))
                {
                    res[login.Kind] = new List<CollectionServerLogin>() { login };
                }
                else
                {
                    ((List<CollectionServerLogin>)res[login.Kind]).Add(login);
                }
            }

            return res;
        }
    }
}