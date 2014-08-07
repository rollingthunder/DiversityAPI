namespace DiversityService.API.Services
{
    using System;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Threading.Tasks;

    public interface IStore<T, TKey> : IDisposable
    {
        Task<IQueryable<T>> FindAsync();   
        Task<T> FindAsync(TKey id); 
        Task InsertAsync(T entity); 
        Task UpdateAsync(T entity); 
        Task DeleteAsync(TKey id);
    }

    public interface ISeriesStore : IStore<Collection.EventSeries, int> { }
    public interface IEventStore : IStore<Collection.Event, int> { }
}
