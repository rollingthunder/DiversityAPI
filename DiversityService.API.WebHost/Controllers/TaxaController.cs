using DiversityService.API.Model;
using DiversityService.API.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DiversityService.API.Controllers
{
    public class TaxaController : DiversityController
    {
        private readonly Lazy<ITaxa> _Taxa;

        private ITaxa Taxa { get { return _Taxa.Value; } }

        private readonly IMappingService Mapping;

        public TaxaController(
            Lazy<ITaxa> Taxa,
            IMappingService Mapping
            )
            : base(Mapping)
        {
            this._Taxa = Taxa;
            this.Mapping = Mapping;
        }

        public IEnumerable<TaxonList> Get(int projectId)
        {
            return from projectList in Taxa.GetTaxonLists(projectId)
                   where projectList.ProjectId == projectId
                   select Mapping.Map<TaxonList>(projectList);
        }
    }
}