namespace DiversityService.API.Model.Internal
{
    using System;

    public class InternalCollectionServer : CollectionServer, IEquatable<InternalCollectionServer>
    {
        public string Address { get; set; }

        public int Port { get; set; }

        public string Catalog { get; set; }

        public bool Equals(InternalCollectionServer other)
        {
            return this.Equals(other as CollectionServer) &&
                this.Address == other.Address &&
                this.Port == other.Port &&
                this.Catalog == other.Catalog;
        }
    }

    public class CollectionServerLogin : InternalCollectionServer, IEquatable<CollectionServerLogin>
    {
        public string User { get; set; }

        public string Password { get; set; }

        public string Kind { get; set; }

        public bool Equals(CollectionServerLogin other)
        {
            return this.Equals(other as InternalCollectionServer) &&
                this.User == other.User &&
                this.Password == other.Password &&
                this.Kind == other.Kind;
        }

        public CollectionServerLogin Clone()
        {
            return (CollectionServerLogin)this.MemberwiseClone();
        }
    }
}