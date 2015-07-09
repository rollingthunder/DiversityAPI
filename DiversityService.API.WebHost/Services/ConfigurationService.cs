namespace DiversityService.API.Services
{
    using AutoMapper;
    using DiversityService.API.Model;
    using DiversityService.API.Model.Internal;
    using DiversityService.API.WebHost;
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Linq;
    using System.Web;

    public class ConfigurationService : IConfigurationService
    {
        private readonly Lazy<CollectionServerConfigurationSection> Section;
        private readonly IMappingEngine Mapper;

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

        private IEnumerable<InternalCollectionServer> _Servers;

        public IEnumerable<InternalCollectionServer> GetCollectionServers()
        {
            LoadConfigurationIfNecessary();

            return _Servers;
        }

        private CollectionServerLogin _PublicTaxa;

        public CollectionServerLogin GetPublicTaxa()
        {
            LoadConfigurationIfNecessary();
            return _PublicTaxa;
        }

        private CollectionServerLogin _ScientificTerms;

        public CollectionServerLogin GetScientificTerms()
        {
            LoadConfigurationIfNecessary();
            return _ScientificTerms;
        }

        #endregion IConfigurationService

        private void LoadConfigurationIfNecessary()
        {
            if (Section.IsValueCreated)
            {
                return;
            }

            var section = Section.Value;

            _PublicTaxa = Mapper.Map<CollectionServerLogin>(section.PublicServers.Taxa);
            _ScientificTerms = Mapper.Map<CollectionServerLogin>(section.PublicServers.Terms);
            _Servers = LoadCollectionServers();
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