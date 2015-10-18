namespace DiversityService.API.Services
{
    using DiversityService.API.Model.Internal;
    using DiversityService.DB.Services;
    using Ninject;
    using System;
    using System.Threading.Tasks;

    public class ContextFactory : IContextFactory
    {
        private readonly IKernel Kernel;

        public ContextFactory(IKernel kernel)
        {
            Kernel = kernel;
        }

        public async Task<IFieldDataContext> CreateContextAsync(CollectionServerLogin server)
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