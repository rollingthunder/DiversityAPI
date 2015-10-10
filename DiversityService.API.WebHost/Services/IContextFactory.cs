namespace DiversityService.API.Services
{
    using DiversityService.API.Model;
    using DiversityService.API.Model.Internal;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Web;

    public interface IContextFactory
    {
        Task<IFieldDataContext> CreateContextAsync(CollectionServerLogin server);
    }
}