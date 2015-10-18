namespace DiversityService.API.Services
{
    using System;
    using System.Threading.Tasks;
    using DiversityService.API.Model.Internal;
    using DiversityService.DB.Services;
    using Ninject;

    public class ContextFactory : IContextFactory
    {
        private readonly IKernel kernel;

        public ContextFactory(IKernel kernel)
        {
            this.kernel = kernel;
        }

        public async Task<IFieldDataContext> CreateContextAsync(CollectionServerLogin server)
        {
            try
            {
                var ctx = DbContextHelpers.CreateCollection(server);

                // Force immediate connection to validate settings 
                await ctx.Database.Connection.OpenAsync();

                return new CollectionContext(
                    kernel,
                    ctx);
            }
            catch (Exception)
            {
                // TODO Log 
                return null;
            }
        }
    }
}