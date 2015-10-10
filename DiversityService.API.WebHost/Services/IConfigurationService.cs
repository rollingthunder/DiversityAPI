namespace DiversityService.API.Services
{
    using DiversityService.API.Model;
    using DiversityService.API.Model.Internal;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public interface IConfigurationService
    {
        IEnumerable<InternalCollectionServer> GetCollectionServers();

        CollectionServerLogin GetPublicLogin(string type);
    }
}