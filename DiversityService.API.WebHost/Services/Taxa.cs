namespace DiversityService.API.Services
{
    using AutoMapper;
    using DiversityService.API.WebHost.Models;
    using DiversityService.Collection;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;

    public interface ITaxa
    {
        IEnumerable<TaxonListForProject> GetTaxonLists(string userName);

        //IQueryable<TaxonName> GetTaxaForList(TaxonListForProject list);
    }

    public class TaxonService : ITaxa
    {
        private readonly Collection Context;

        public TaxonService(
            Collection context
            )
        {
            this.Context = context;
        }

        public IEnumerable<TaxonListForProject> GetTaxonLists(string userName)
        {
            return null;

            //(from list in Collection.DiversityMobile_UserTaxonLists(User.BackendUser)
            //    where list.ProjectID == projectId
            //    select Mapping.Map<TaxonListForProject>(list))
            //   .ToList();
        }
    }
}