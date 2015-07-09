namespace DiversityService.API.Controllers
{
    using AutoMapper;
    using DiversityService.API.Filters;
    using DiversityService.API.Model;
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

        public TaxaController(
            IMappingEngine Mapping
            )
            : base(Mapping)
        {
            this.Mapping = Mapping;
        }

        public async Task<IEnumerable<TaxonList>> Get()
        {
            return null;
        }

        [Route("{listID}")]
        public async Task<IEnumerable<TaxonName>> GetList(int listID, int take = 10, int skip = 0)
        {
            var knownLists = await Get();

            return null;
        }
    }
}