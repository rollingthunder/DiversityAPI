namespace DiversityService.API.Controllers
{
    using DiversityService.API.Filters;
    using DiversityService.API.Model;
    using DiversityService.API.Services;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Threading.Tasks;
    using System.Web.Http;

    [Authorize]
    [RequireCollection]
    public class ProjectController : ApiController
    {
        private readonly IProjectStore ProjectStore;

        public ProjectController(
            IProjectStore projectStore
            )
        {
            ProjectStore = projectStore;
        }

        public Task<IEnumerable<Project>> Get()
        {
            return ProjectStore.GetProjectsAsync();
        }
    }
}