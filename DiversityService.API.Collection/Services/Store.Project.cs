namespace DiversityService.API.Services
{
    using DiversityService.DB.Collection;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Threading.Tasks;

    public interface IProjectStore : IReadOnlyStore<Project, int>
    {
    }

    public class ProjectStore : IProjectStore
    {
        private readonly DiversityCollection Context;

        public ProjectStore(
            DiversityCollection context
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

            return await Store.Get(
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