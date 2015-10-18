namespace DiversityService.API.Controllers
{
    using AutoMapper;
    using DiversityService.API.Filters;
    using DiversityService.API.Model;
    using DiversityService.API.Services;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Web.Http;

    [CollectionAPI("project")]
    public class ProjectController : ApiController
    {
        private IProjectStore ProjectStore
        {
            get
            {
                var ctx = Request.GetCollectionContext();

                return ctx.Projects;
            }
        }

        private readonly IMappingEngine Mapper;

        public ProjectController(
            IMappingEngine mapper
            )
        {
            Mapper = mapper;
        }

        [Route]
        public async Task<IEnumerable<Project>> Get()
        {
            var projects = await ProjectStore.GetAsync();

            return Mapper.Map<IEnumerable<Project>>(projects);
        }

        [Route(CollectionAPI.PROJECT_TEMPLATE)]
        public async Task<Project> GetById(int project)
        {
            var projects = await Get();

            return projects
                .Where(x => x.Id == project)
                .FirstOrDefault();
        }
    }
}