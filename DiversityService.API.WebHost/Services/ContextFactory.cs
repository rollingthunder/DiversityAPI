namespace DiversityService.API.Services
{
    using DiversityService.API.Model;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Web;

    public class ContextFactory : IContextFactory
    {
        public Task<IContext> CreateContextAsync(InternalCollectionServer server, string user, string password)
        {
            throw new NotImplementedException();
        }
    }
}