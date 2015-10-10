namespace DiversityService.API.Controllers
{
    using Akavache;
    using AutoMapper;
    using DiversityService.API.Filters;
    using DiversityService.API.Model;
    using Model.Internal;
    using DiversityService.API.Services;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Web;
    using System.Web.Http;

    [CollectionAPI(Route.TAXA_CONTROLLER)]
    public class TaxaController : DiversityController
    {
        private readonly IMappingEngine Mapping;
        private readonly IConfigurationService Configuration;
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
            var knownLists = await Get();

            return await getTaxonList(Login, knownLists, listID, take, skip);
        }

        [Route("public")]
        public Task<IEnumerable<TaxonList>> GetPublic()
        {
            return null;
        }

        [Route("public/{listID}")]
        public async Task<IEnumerable<TaxonName>> GetPublicList(string listID, int take = 10, int skip = 0)
        {
            var knownLists = await GetPublic();

            return await getTaxonList(Login, knownLists, listID, take, skip);
        }

        
        private async Task<IEnumerable<TaxonName>> getTaxonList(CollectionServerLogin login, IEnumerable<TaxonList> knownLists, string listId, int take, int skip)
        {
            return null;

        }

        private async Task<IEnumerable<TaxonList>> enumerateTaxonListsForServer(CollectionServerLogin login)
        {
            var taxaModules = from module in await ModuleDiscovery.DiscoverModules(Login)
                              where module.Type == DBModuleType.TaxonNames
                              select module;

            var result = new List<TaxonList>();

            foreach (var tm in taxaModules)
            {
                // Create login for this module
                var taxaLogin = login.Clone();
                taxaLogin.Catalog = tm.Catalog;

                var taxa = TaxaFactory.GetTaxa(taxaLogin);

                var lists = from list in await taxa.GetListsForUserAsync()
                            select Mapper.Map<TaxonList>(list);

                result.AddRange(lists);
            }

            return result;
        }
    }
}