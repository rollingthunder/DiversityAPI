namespace DiversityService.API.Services
{
    using DiversityService.API.Model;
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Diagnostics.Contracts;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Web;

    public interface IProjectStore
    {
        Task<bool> IsValidProjectAsync(int projectId);

        Task<IEnumerable<Project>> GetProjectsAsync();
    }

    public class ProjectStore : IProjectStore
    {
        private readonly CollectionContext Context;
        private readonly IMappingService Mapper;

        public ProjectStore(
            IContext context,
            IMappingService mapper
            )
        {
            Contract.Requires<ArgumentException>(context is CollectionContext);
            Contract.Requires<ArgumentNullException>(mapper != null);

            Context = context as CollectionContext;
            Mapper = mapper;
        }

        public Task<bool> IsValidProjectAsync(int projectId)
        {
            return (from project in Context.DiversityMobile_ProjectList()
                    where project.ProjectID == projectId
                    select project).AnyAsync();
        }

        public async Task<IEnumerable<Project>> GetProjectsAsync()
        {
            var projects = await Context
                .DiversityMobile_ProjectList()
                .ToListAsync();

            return from project in projects
                   select Mapper.Map<Project>(project);
        }
    }
}