namespace DiversityService.API.Model
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public partial class CollectionServer : IEquatable<CollectionServer>
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public bool Equals(CollectionServer other)
        {
            return other != null &&
                this.Id == other.Id &&
                this.Name == other.Name;
        }
    }
}