namespace DiversityService.API.Services
{
    using DiversityService.Collection;
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Diagnostics.Contracts;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Threading.Tasks;
    using System.Web;

    public interface IProjectStore : IReadOnlyStore<Project, int>
    {
    }

    public class ProjectStore : IProjectStore
    {
        private readonly Collection Context;

        public ProjectStore(
            Collection context
            )
        {
            Context = context;
        }

        public async Task<IEnumerable<Project>> GetAsync(
            Expression<Func<Project, bool>> filter = null,
            Func<IQueryable<Project>, IQueryable<Project>> restrict = null,
            Func<IQueryable<Project>, IOrderedQueryable<Project>> orderBy = null,
            string includeProperties = "")
        {
            var query = await GetQueryableAsync();

            return await StoreComponents.Get(
                query,
                filter,
                restrict,
                orderBy,
                includeProperties
            );
        }

        public async Task<Project> GetByIDAsync(int id)
        {
            var query = await GetAsync(
                x => x.ProjectID == id
                );
            return query.FirstOrDefault();
        }

        public async Task<IQueryable<Project>> GetQueryableAsync()
        {
            return Context
                .DiversityMobile_ProjectList();
        }
    }
}