namespace DiversityService.API.Model.Internal
{
    public enum DBModuleType
    {
        Unknown,
        TaxonNames,
        Collection
    }

    public class DBModule
    {
        public DBModuleType Type { get; private set; }

        public string Catalog { get; private set; }

        public DBModule(DBModuleType type, string catalog)
        {
            Type = type;
            Catalog = catalog;
        }
    }
}