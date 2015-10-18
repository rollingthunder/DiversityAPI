namespace DiversityService.API.Services
{
    using DiversityService.API.Model.Internal;
    using System.Collections.Generic;

    public interface IConfigurationService
    {
        IEnumerable<InternalCollectionServer> GetCollectionServers();

        CollectionServerLogin GetPublicLogin(string type);
    }
}