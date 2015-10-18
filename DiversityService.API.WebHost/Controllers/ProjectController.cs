namespace DiversityService.API.Controllers
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Web.Http;
    using AutoMapper;
    using DiversityService.API.Filters;
    using DiversityService.API.Model;
    using DiversityService.API.Services;

    [CollectionAPI("project")]
    public class ProjectController : ApiController
    {
        public ProjectController(
             IMappingEngine mapper)
        {
            Mapper = mapper;
        }

        private IMappingEngine Mapper { get; set; }

        private IProjectStore ProjectStore
        {
            get
            {
                var ctx = Request.GetCollectionContext();

                return ctx.Projects;
            }
        }

        [Route]
        public async Task<IEnumerable<Project>> Get()
        {
            var projects = await ProjectStore.GetAsync();

            return Mapper.Map<IEnumerable<Project>>(projects);
        }

        [Route(CollectionAPI.ProjectTemplate)]
        public async Task<Project> GetById(int project)
        {
            var projects = await Get();

            return projects
                .Where(x => x.Id == project)
                .FirstOrDefault();
        }
    }
}