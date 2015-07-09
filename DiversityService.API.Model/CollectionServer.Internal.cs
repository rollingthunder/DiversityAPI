namespace DiversityService.API.Model.Internal
{
    public class InternalCollectionServer : CollectionServer
    {
        public string Address { get; set; }

        public int Port { get; set; }

        public string Catalog { get; set; }
    }

    public class CollectionServerLogin : InternalCollectionServer
    {
        public string User { get; set; }

        public string Password { get; set; }
    }
}