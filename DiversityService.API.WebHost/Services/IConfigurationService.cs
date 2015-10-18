namespace DiversityService.API.Services
{
    using System.Collections.Generic;
    using DiversityService.API.Model.Internal;

    public interface IConfigurationService
    {
        IEnumerable<InternalCollectionServer> GetCollectionServers();

        CollectionServerLogin GetPublicLogin(string type);
    }
}