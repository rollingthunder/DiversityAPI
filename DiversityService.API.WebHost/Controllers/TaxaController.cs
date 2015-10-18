namespace DiversityService.API.Controllers
{
    using Akavache;
    using AutoMapper;
    using DiversityService.API.Filters;
    using DiversityService.API.Model;
    using DiversityService.API.Services;
    using Model.Internal;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Web.Http;

    [CollectionAPI(Route.TAXA_CONTROLLER)]
    public class TaxaController : DiversityController
    {
        public const string TAXON_LOGIN_KIND = "taxa";

        private readonly IMappingEngine Mapping;
        private new readonly IConfigurationService Configuration;
        private readonly IDiscoverDBModules ModuleDiscovery;
        private readonly ITaxaFactory TaxaFactory;

        private IObjectBlobCache _Cache;

        public IObjectBlobCache Cache
        {
            get { return _Cache ?? BlobCache.InMemory as IObjectBlobCache; }
            set { _Cache = value; }
        }

        private CollectionServerLogin Login { get { return Request.GetBackendCredentials(); } }

        public TaxaController(
            IMappingEngine Mapping,
            IConfigurationService Configuration,
            IDiscoverDBModules ModuleDiscovery,
            ITaxaFactory TaxaFactory)
            : base(Mapping)
        {
            this.Mapping = Mapping;
            this.Configuration = Configuration;
            this.ModuleDiscovery = ModuleDiscovery;
            this.TaxaFactory = TaxaFactory;
        }

        /// <summary>
        /// Get taxon lists from the server we are currently connected to
        /// </summary>
        /// <returns></returns>
        public Task<IEnumerable<TaxonList>> Get()
        {
            return this.enumerateTaxonListsForServer(Login);
        }

        [Route("{listID}")]
        public async Task<IEnumerable<TaxonName>> GetList(string listID, int take = 10, int skip = 0)
        {
            var knownLists = await this.Get();

            return await getTaxonList(Login, listID, take, skip);
        }

        [Route("public")]
        public async Task<IEnumerable<TaxonList>> GetPublic()
        {
            var publicLogin = Configuration.GetPublicLogin(TAXON_LOGIN_KIND);

            return await enumerateTaxonListsForServer(publicLogin);
        }

        [Route("public/{listID}")]
        public async Task<IEnumerable<TaxonName>> GetPublicList(string listID, int take = 10, int skip = 0)
        {
            return await getTaxonList(Login, listID, take, skip);
        }

        private async Task<IEnumerable<TaxonName>> getTaxonList(CollectionServerLogin login, string listId, int take, int skip)
        {
            var known = await discoverModulesAndLists(login);

            ServerTaxonList list;

            if (!known.TryGetValue(listId, out list))
            {
                return Enumerable.Empty<TaxonName>();
            }

            var taxonLogin = login.Clone();
            taxonLogin.Catalog = list.Module.Catalog;

            var taxa = this.TaxaFactory.GetTaxa(taxonLogin);

            return from name in await taxa.GetTaxonNamesForList(list.DatabaseId, skip, take)
                   select Mapper.Map<TaxonName>(name);
        }

        private async Task<IEnumerable<TaxonList>> enumerateTaxonListsForServer(CollectionServerLogin login)
        {
            var listByHash = await discoverModulesAndLists(login);
            return listByHash.Values;
        }

        private async Task<IDictionary<string, ServerTaxonList>> discoverModulesAndLists(CollectionServerLogin login)
        {
            var result = new Dictionary<string, ServerTaxonList>();

            var taxaModules = from module in await this.ModuleDiscovery.DiscoverModules(this.Login)
                              where module.Type == DBModuleType.TaxonNames
                              select module;

            foreach (var tm in taxaModules)
            {
                // Create login for this module
                var taxaLogin = login.Clone();
                taxaLogin.Catalog = tm.Catalog;

                var taxa = TaxaFactory.GetTaxa(taxaLogin);

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
    }
}