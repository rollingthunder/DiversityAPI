using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DiversityService.API.WebHost.Services
{
    public class CollectionContext : Collection.Collection, IContext
    {
        public CollectionContext(string connectionString) : base(connectionString)
        {

        }
    }

    public class UnitOfWork : IUnitOfWork
    {
        private readonly IContext context;

        public UnitOfWork(IContext context)
        {
            this.context = context;
        }
        public int Save()
        {
            return context.SaveChanges();
        }
        public IContext Context
        {
            get { return context; }
        }
        public void Dispose()
        {
            if (context != null)
            {
                context.Dispose();
            }
        }
    }
}