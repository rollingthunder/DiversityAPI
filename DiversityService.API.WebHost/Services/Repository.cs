namespace DiversityService.API.Services
{
    using DiversityService.Collection;
    using System.Data.Entity;
    using System.Linq;
    using System.Threading.Tasks;

    public class Store<T> : IStore<T, int> where T : class, DiversityService.API.Model.IIdentifiable
    {     
        private readonly CollectionContext Context;

        public Store(CollectionContext ctx)
        {
            Context = ctx;
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
            if (Context != null)
            {
                Context.Dispose();
            }
        }
    }

    public class SeriesStore : Store<EventSeries>, ISeriesStore
    {
        public SeriesStore(CollectionContext ctx) : base(ctx) {}
    }
}