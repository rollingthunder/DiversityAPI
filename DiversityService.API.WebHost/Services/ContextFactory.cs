namespace DiversityService.API.Services
{
    using DiversityService.API.Model;
    using Ninject;
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Web;

    public class ContextFactory : IContextFactory
    {
        private IKernel Kernel;

        public async Task<IContext> CreateContextAsync(InternalCollectionServer server, string user, string password)
        {
            try
            {
                var ctx = new Collection.Collection(GetConnectionString(server, user, password));

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

        private static string GetConnectionString(InternalCollectionServer server, string user, string password)
        {
            return string.Format(
                ConfigurationManager.ConnectionStrings["Collection"].ConnectionString,
                server.Address,
                server.Port,
                server.Catalog,
                user,
                password
            );
        }
    }
}