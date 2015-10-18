namespace DiversityService.API.Model.Internal
{
    public class ServerTaxonList : TaxonList
    {
        public int DatabaseId
        {
            get; set;
        }

        public DBModule Module
        {
            get; set;
        }
    }
}