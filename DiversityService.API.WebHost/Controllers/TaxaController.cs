namespace DiversityService.API.Controllers
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Web.Http;
    using Akavache;
    using AutoMapper;
    using DiversityService.API.Filters;
    using DiversityService.API.Model;
    using DiversityService.API.Services;
    using Model.Internal;

    [CollectionAPI(Route.TaxaController)]
    public class TaxaController : DiversityController
    {
        public const string TaxonLoginKind = "taxa";

        private readonly IConfigurationService configuration;
        private readonly IDiscoverDBModules moduleDiscovery;
        private readonly ITaxaFactory taxaFactory;

        private IObjectBlobCache cache;

        public TaxaController(
                  IMappingEngine mapping,
                  IConfigurationService configuration,
                  IDiscoverDBModules moduleDiscovery,
                  ITaxaFactory taxaFactory)
                  : base(mapping)
        {
            this.configuration = configuration;
            this.moduleDiscovery = moduleDiscovery;
            this.taxaFactory = taxaFactory;
        }

        public IObjectBlobCache Cache
        {
            get
            {
                return cache ?? BlobCache.InMemory as IObjectBlobCache;
            }

            set
            {
                cache = value;
            }
        }

        private CollectionServerLogin Login
        {
            get
            {
                return Request.GetBackendCredentials();
            }
        }

        /// <summary>
        /// Get taxon lists from the server we are currently connected to 
        /// </summary>
        /// <returns></returns>
        public Task<IEnumerable<TaxonList>> Get()
        {
            return this.EnumerateTaxonListsForServer(Login);
        }

        [Route("{listID}")]
        public async Task<IEnumerable<TaxonName>> GetList(string listID, int take = 10, int skip = 0)
        {
            var knownLists = await this.Get();

            return await GetTaxonList(Login, listID, take, skip);
        }

        [Route("public")]
        public async Task<IEnumerable<TaxonList>> GetPublic()
        {
            var publicLogin = configuration.GetPublicLogin(TaxonLoginKind);

            return await EnumerateTaxonListsForServer(publicLogin);
        }

        [Route("public/{listID}")]
        public async Task<IEnumerable<TaxonName>> GetPublicList(string listID, int take = 10, int skip = 0)
        {
            var publicLogin = configuration.GetPublicLogin(TaxonLoginKind);

            return await GetTaxonList(publicLogin, listID, take, skip);
        }

        private async Task<IDictionary<string, ServerTaxonList>> DiscoverModulesAndLists(CollectionServerLogin login)
        {
            var result = new Dictionary<string, ServerTaxonList>();

            var taxaModules = from module in await this.moduleDiscovery.DiscoverModules(this.Login)
                              where module.Type == DBModuleType.TaxonNames
                              select module;

            foreach (var tm in taxaModules)
            {
                // Create login for this module 
                var taxaLogin = login.Clone();
                taxaLogin.Catalog = tm.Catalog;

                var taxa = taxaFactory.GetTaxa(taxaLogin);

                var lists = await taxa.GetListsForUserAsync();

                foreach (var l in lists)
                {
                    var mapped = Mapper.Map<ServerTaxonList>(l);
                    mapped.Module = tm;
                    result.Add(mapped.Id, mapped);
                }
            }

            // TODO Cache results 
            return result;
        }

        private async Task<IEnumerable<TaxonList>> EnumerateTaxonListsForServer(CollectionServerLogin login)
        {
            var listByHash = await DiscoverModulesAndLists(login);
            return listByHash.Values;
        }

        private async Task<IEnumerable<TaxonName>> GetTaxonList(CollectionServerLogin login, string listId, int take, int skip)
        {
            var known = await DiscoverModulesAndLists(login);

            ServerTaxonList list;

            if (!known.TryGetValue(listId, out list))
            {
                return Enumerable.Empty<TaxonName>();
            }

            var taxonLogin = login.Clone();
            taxonLogin.Catalog = list.Module.Catalog;

            var taxa = taxaFactory.GetTaxa(taxonLogin);

            return from name in await taxa.GetTaxonNamesForList(list.DatabaseId, skip, take)
                   select Mapper.Map<TaxonName>(name);
        }
    }
}