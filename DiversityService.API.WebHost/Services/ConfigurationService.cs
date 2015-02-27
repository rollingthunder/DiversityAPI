namespace DiversityService.API.Services
{
    using AutoMapper;
    using DiversityService.API.Model;
    using DiversityService.API.WebHost;
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Linq;
    using System.Web;

    public class ConfigurationService : IConfigurationService
    {
        private readonly Lazy<IEnumerable<InternalCollectionServer>> Servers;
        private readonly IMappingEngine Mapper;

        public ConfigurationService(
            IMappingEngine mapper
            )
        {
            Mapper = mapper;

            Servers = new Lazy<IEnumerable<InternalCollectionServer>>(LoadCollectionServers);
        }

        public IEnumerable<InternalCollectionServer> GetCollectionServers()
        {
            return Servers.Value;
        }

        private IEnumerable<InternalCollectionServer> LoadCollectionServers()
        {
            var serverSection = (CollectionServerConfigurationSection)ConfigurationManager.GetSection("collectionServers");
            var collectionServers = serverSection.Servers.Cast<CollectionServerElement>()
                .Select(Mapper.Map<InternalCollectionServer>)
                .ToList();

            // Throws if there are duplicate Ids
            collectionServers.ToDictionary(x => x.Id);

            return collectionServers;
        }
    }
}