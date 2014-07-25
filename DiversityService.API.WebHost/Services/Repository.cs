namespace DiversityService.API.WebHost.Services
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Threading.Tasks;
    using System.Web;

    public class Repository<T> : IRepository<T> where T : class, IIdentifiable
    {
        private readonly IUnitOfWork transaction;
        private readonly CollectionContext context;
        public Repository(IUnitOfWork uow)
        {
            transaction = uow;
            context = uow.Context as CollectionContext;
        }
        public IUnitOfWork Transaction
        {
            get
            {
                return transaction;
            }
        }
        public IQueryable<T> All
        {
            get
            {
                return context.Set<T>();
            }
        }
        public IQueryable<T> AllEager(params Expression<Func<T, object>>[] includes)
        {
            IQueryable<T> query = context.Set<T>();
            foreach (var include in includes)
            {
                query = query.Include(include);
            }
            return query;
        }
        public Task<T> Find(int id)
        {
            return context.Set<T>().FindAsync(id);
        }
        public void Insert(T item)
        {
            context.Entry(item).State = EntityState.Added;
        }
        public void Update(T item)
        {
            context.Set<T>().Attach(item); context.Entry(item).State = EntityState.Modified;
        }
        public async Task Delete(int id)
        {
            var item = await context.Set<T>().FindAsync(id); 
            context.Set<T>().Remove(item);
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