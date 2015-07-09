namespace DiversityService.API.Services
{
    using DiversityService.API.Model;
    using DiversityService.API.Model.Internal;
    using DiversityService.DB.Collection;
    using DiversityService.DB.Services;
    using Ninject;
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Web;

    public class ContextFactory : IContextFactory
    {
        private readonly IKernel Kernel;

        public ContextFactory(IKernel kernel)
        {
            Kernel = kernel;
        }

        public async Task<IContext> CreateContextAsync(CollectionServerLogin server)
        {
            try
            {
                var ctx = DbContextHelpers.CreateCollection(server);

                // Force immediate connection to validate settings
                await ctx.Database.Connection.OpenAsync();

                return new CollectionContext(
                    Kernel,
                    ctx
                );
            }
            catch (Exception ex)
            {
                // TODO Log
                return null;
            }
        }
    }
}