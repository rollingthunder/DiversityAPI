namespace DiversityService.API.Services
{
    using DiversityService.API.Model;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Threading.Tasks;

    public interface IReadOnlyStore<TEntity, TKey>
    {
        Task<IEnumerable<TEntity>> GetAsync(
            Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IQueryable<TEntity>> restrict = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            string includeProperties = "");

        Task<IQueryable<TEntity>> GetQueryableAsync();

        Task<TEntity> GetByIDAsync(TKey id);
    }

    public interface IStore<TEntity, TKey> : IReadOnlyStore<TEntity, TKey>
    {
        Task InsertAsync(TEntity entity);

        Task DeleteAsync(TKey id);

        Task DeleteAsync(TEntity entityToDelete);

        Task UpdateAsync(TEntity entityToUpdate);
    }

    public interface ISeriesStore : IStore<Collection.EventSeries, int> { }

    public interface IEventStore : IStore<Collection.Event, int> { }
}