namespace DiversityService.API.Controllers
{
    using AutoMapper;
    using DiversityService.API.Filters;
    using DiversityService.API.Model;
    using DiversityService.API.Services;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;

    [ProjectAPI(Route.TAXA_CONTROLLER)]
    public class TaxaController : DiversityController
    {
        private ITaxa Taxa
        {
            get
            {
                return Request.GetCollectionContext().Taxa;
            }
        }

        private readonly IMappingEngine Mapping;

        public TaxaController(
            IMappingEngine Mapping
            )
            : base(Mapping)
        {
            this.Mapping = Mapping;
        }

        public IEnumerable<TaxonList> Get()
        {
            return null;
        }
    }
}