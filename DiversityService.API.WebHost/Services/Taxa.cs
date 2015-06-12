namespace DiversityService.API.Services
{
    using AutoMapper;
    using DiversityService.API.Model;
    using DiversityService.API.WebHost.Models;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using Collection = DiversityService.DB.Collection;

    public interface ITaxa
    {
        IEnumerable<TaxonList> GetTaxonLists(string userName);

        //IQueryable<TaxonName> GetTaxaForList(TaxonList list);
    }

    public class TaxonService : ITaxa
    {
        private readonly Collection.DiversityCollection Context;

        public TaxonService(
            Collection.DiversityCollection context
            )
        {
            this.Context = context;
        }

        public IEnumerable<TaxonList> GetTaxonLists(string userName)
        {
            return null;

            //(from list in DiversityCollection.DiversityMobile_UserTaxonLists(User.BackendUser)
            //    where list.ProjectID == projectId
            //    select Mapping.Map<TaxonList>(list))
            //   .ToList();
        }
    }
}