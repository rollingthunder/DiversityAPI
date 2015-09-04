namespace DiversityService.DB.Services
{
    using DiversityService.API;
    using DiversityService.API.Model.Internal;
    using DiversityService.DB.TaxonNames;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public class Taxa : ITaxa
    {
        private readonly TaxonNames Ctx;

        public Taxa(CollectionServerLogin Login)
        {
            Ctx = new TaxonNames();
        }

        public Task<IEnumerable<TaxonList>> GetListsForUserAsync()
        {
            throw new System.NotImplementedException();
        }

        public Task<IList<TaxonName>> GetTaxonNamesForList(int ListID, int skip = 0, int take = 50)
        {
            throw new System.NotImplementedException();
        }
    }
}