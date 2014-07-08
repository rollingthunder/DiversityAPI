using DiversityService.API.Model;
using Ninject.Modules;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using AutoMapper;
using DiversityService.API.WebHost.Controllers;

namespace DiversityService.API.WebHost
{
    public class ConfigurationModule : NinjectModule
    {
        private IEnumerable<CollectionServer> LoadServerConfiguration(Ninject.Activation.IContext ctx)
        {
            var serverSection = (CollectionServerConfigurationSection)ConfigurationManager.GetSection("collectionServers");
            var collectionServers = serverSection.Servers.Cast<CollectionServerElement>()
                .Select(Mapper.Map<CollectionServer>)
                .ToList();

            // Throws if there are duplicate Ids
            collectionServers.ToDictionary(x => x.Id);

            return collectionServers;
        }

        public override void Load()
        {
            this.Bind<IEnumerable<CollectionServer>>()
                .ToMethod(LoadServerConfiguration)
                .InSingletonScope();
                
        }
    }
}