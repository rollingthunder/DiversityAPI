namespace DiversityService.API.Services
{
    using DiversityService.Collection;
    using Ninject;
    using System;
    using System.Data.Entity;
    using System.Linq;
    using System.Threading.Tasks;

    public class Store<T> : IStore<T, int> where T : class, DiversityService.API.Model.IIdentifiable
    {
        public Store(Func<IContext> ContextFactory)
        {
            _Context = new Lazy<CollectionContext>(() => ContextFactory() as CollectionContext);
        }

        private Lazy<CollectionContext> _Context;

        private CollectionContext Context
        {
            get
            {
                return _Context.Value;
            }
        }

        public async Task<IQueryable<T>> FindAsync()
        {
            return Context.Set<T>();
        }

        public Task<T> FindAsync(int id)
        {
            return Context.Set<T>().FindAsync(id);
        }

        public async Task InsertAsync(T item)
        {
            Context.Entry(item).State = EntityState.Added;
        }

        public async Task UpdateAsync(T item)
        {
            Context.Set<T>().Attach(item); Context.Entry(item).State = EntityState.Modified;
        }

        public async Task DeleteAsync(int id)
        {
            var item = await Context.Set<T>().FindAsync(id);
            Context.Set<T>().Remove(item);
        }

        public void Dispose()
        {
            if (_Context.IsValueCreated && Context != null)
            {
                Context.Dispose();
            }
        }
    }

    public class SeriesStore : Store<EventSeries>, ISeriesStore
    {
        public SeriesStore(Func<CollectionContext> ContextFactory)
            : base(ContextFactory)
        {
        }
    }
}