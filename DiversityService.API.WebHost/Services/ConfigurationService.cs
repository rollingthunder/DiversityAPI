namespace DiversityService.API.Services
{
    using AutoMapper;
    using DiversityService.API.Model.Internal;
    using DiversityService.API.WebHost;
    using Splat;
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Linq;

    public class ConfigurationService : IConfigurationService, IEnableLogger
    {
        private readonly Lazy<CollectionServerConfigurationSection> Section;
        private readonly IMappingEngine Mapper;

        private IEnumerable<InternalCollectionServer> _Servers;

        private IDictionary<string, IEnumerable<CollectionServerLogin>> _PublicServers;

        public ConfigurationService(
            IMappingEngine mapper
            )
        {
            Mapper = mapper;

            Section = new Lazy<CollectionServerConfigurationSection>(() =>
            {
                return (CollectionServerConfigurationSection)ConfigurationManager.GetSection("collectionServers");
            });
        }

        #region IConfigurationService

        public IEnumerable<InternalCollectionServer> GetCollectionServers()
        {
            LoadConfigurationIfNecessary();

            return _Servers;
        }

        public CollectionServerLogin GetPublicLogin(string kind)
        {
            LoadConfigurationIfNecessary();

            IEnumerable<CollectionServerLogin> Logins;
            if (_PublicServers.TryGetValue(kind, out Logins))
            {
                return Logins.First();
            }

            return null;
        }

        #endregion IConfigurationService

        private void LoadConfigurationIfNecessary()
        {
            if (Section.IsValueCreated)
            {
                return;
            }

            _PublicServers = LoadPublicServers();
            _Servers = LoadCollectionServers();
        }

        private IDictionary<string, IEnumerable<CollectionServerLogin>> LoadPublicServers()
        {
            var res = new Dictionary<string, IEnumerable<CollectionServerLogin>>();

            foreach (var server in Section.Value.PublicServers.Cast<ServerLoginCatalogElement>())
            {
                var login = Mapper.Map<CollectionServerLogin>(server);

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

        private IEnumerable<InternalCollectionServer> LoadCollectionServers()
        {
            var collectionServers = Section.Value.Servers.Cast<CollectionServerElement>()
                .Select(Mapper.Map<InternalCollectionServer>)
                .ToList();

            // Throws if there are duplicate Ids
            collectionServers.ToDictionary(x => x.Id);

            return collectionServers;
        }
    }
}