namespace DiversityService.API
{
    using DiversityService.API.Model;
    using DiversityService.API.Model.Internal;
    using TaxonNames = DB.TaxonNames;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface ITaxaFactory
    {
        ITaxa GetTaxa(CollectionServerLogin login);
    }

    public interface ITaxa
    {
        Task<IEnumerable<TaxonNames.TaxonList>> GetListsForUserAsync();

        Task<IList<TaxonNames.TaxonName>> GetTaxonNamesForList(int ListID, int skip = 0, int take = 50);
    }
}