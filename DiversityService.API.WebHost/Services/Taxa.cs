namespace DiversityService.API.Services
{
    using DiversityService.API.WebHost.Models;
    using DiversityService.Collection;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;

    public interface ITaxa
    {
        IEnumerable<TaxonListForProject> GetTaxonLists(int projectId);

        //IQueryable<TaxonName> GetTaxaForList(TaxonListForProject list);
    }

    public class TaxonService : ITaxa
    {
        private readonly IMappingService Mapping;
        private readonly CollectionContext Collection;
        private readonly ApplicationUser User;

        public TaxonService(
            IMappingService Mapping,
            CollectionContext Collection,
            ApplicationUser user
            )
        {
            this.Mapping = Mapping;
            this.Collection = Collection;
            this.User = user;
        }

        public IEnumerable<TaxonListForProject> GetTaxonLists(int projectId)
        {
            return null;

            //(from list in Collection.DiversityMobile_UserTaxonLists(User.BackendUser)
            //    where list.ProjectID == projectId
            //    select Mapping.Map<TaxonListForProject>(list))
            //   .ToList();
        }
    }
}