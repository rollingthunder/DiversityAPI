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

    [RequireCollection]
    [RoutePrefix(CollectionAPI.COLLECTION_PREFIX + "project")]
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

        private readonly IMappingService Mapper;

        public ProjectController(
            IMappingService mapper
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
    }
}