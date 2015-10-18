namespace DiversityService.API.Model.Internal
{
    public class ServerTaxonList : TaxonList
    {
        public DBModule Module { get; set; }

        public int DatabaseId { get; set; }
    }
}