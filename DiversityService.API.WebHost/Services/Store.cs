﻿namespace DiversityService.API.Services
{
    using DiversityService.API.Model;
    using DiversityService.Collection;
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Threading.Tasks;

    public static class StoreComponents
    {
        public static async Task<IEnumerable<TEntity>> Get<TEntity>(
            IQueryable<TEntity> query,
            Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IQueryable<TEntity>> restrict = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            string includeProperties = "")
        {
            if (filter != null)
            {
                query = query.Where(filter);
            }

            foreach (var includeProperty in includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                query = query.Include(includeProperty);
            }

            if (restrict != null)
            {
                query = restrict(query);
            }

            if (orderBy != null)
            {
                query = orderBy(query);
            }

            return await query.ToListAsync();
        }
    }

    public class Store<TEntity, TKey> : IStore<TEntity, TKey> where TEntity : class
    {
        private readonly Collection context;
        private readonly DbSet<TEntity> dbSet;

        public Store(Collection context)
        {
            this.context = context;
            this.dbSet = context.Set<TEntity>();
        }

        public virtual async Task<IEnumerable<TEntity>> GetAsync(
           Expression<Func<TEntity, bool>> filter = null,
           Func<IQueryable<TEntity>, IQueryable<TEntity>> restrict = null,
           Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
           string includeProperties = "")
        {
            IQueryable<TEntity> query = dbSet;

            return await StoreComponents.Get(query, filter, restrict, orderBy, includeProperties);
        }

        public virtual Task<TEntity> GetByIDAsync(TKey id)
        {
            if (typeof(ICompositeKey).IsAssignableFrom(typeof(TKey)))
            {
                var key = id as ICompositeKey;
                return dbSet.FindAsync(key.Values());
            }
            return dbSet.FindAsync(id);
        }

        public virtual async Task InsertAsync(TEntity entity)
        {
            dbSet.Add(entity);
            await context.SaveChangesAsync();
        }

        public virtual async Task DeleteAsync(TKey id)
        {
            TEntity entityToDelete = await dbSet.FindAsync(id);
            await DeleteAsync(entityToDelete);
        }

        public virtual async Task DeleteAsync(TEntity entityToDelete)
        {
            if (context.Entry(entityToDelete).State == EntityState.Detached)
            {
                dbSet.Attach(entityToDelete);
            }
            dbSet.Remove(entityToDelete);
            await context.SaveChangesAsync();
        }

        public virtual async Task UpdateAsync(TEntity entityToUpdate)
        {
            dbSet.Attach(entityToUpdate);
            context.Entry(entityToUpdate).State = EntityState.Modified;
            await context.SaveChangesAsync();
        }

        public async Task<IQueryable<TEntity>> GetQueryableAsync()
        {
            return dbSet;
        }
    }
}