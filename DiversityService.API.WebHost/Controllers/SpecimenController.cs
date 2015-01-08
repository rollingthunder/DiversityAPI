namespace DiversityService.API.Controllers
{
    using AutoMapper;
    using DiversityService.API.Filters;
    using DiversityService.API.Model;
    using DiversityService.API.Services;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Web.Http;

    [CollectionAPI(Route.SPECIMEN_CONTROLLER)]
    public class SpecimenController : DiversityController, IFieldDataController<SpecimenBindingModel>
    {
        private readonly IMappingService Mapper;

        private IStore<Collection.Specimen, int> Store
        {
            get
            {
                return Request.GetCollectionContext().Specimen;
            }
        }

        public SpecimenController(
            IMappingService mapper
            )
            : base(mapper)
        {
            this.Mapper = mapper;
        }

        [Route]
        public async Task<IHttpActionResult> Get()
        {
            var all = await Store.GetQueryableAsync();

            var query = all.OrderBy(x => x.Id);

            return PagedAndMapped<Collection.Specimen, Specimen>(query);
        }

        [Route("{id}")]
        public async Task<IHttpActionResult> Get(int id)
        {
            var x = await Store.GetByIDAsync(id);

            if (x == null)
            {
                return NotFound();
            }

            var dto = Mapper.Map<Specimen>(x);

            return Ok(dto);
        }

        [Route]
        public async Task<IHttpActionResult> Post(SpecimenBindingModel value)
        {
            var existing = await RedirectToExisting(Store, value.TransactionGuid);

            if (existing != null)
            {
                return existing;
            }

            var newEntity = Mapper.Map<Collection.Specimen>(value);

            await Store.InsertAsync(newEntity);

            return CreatedAtRoute(Route.DEFAULT_API, Route.GetById(newEntity), newEntity.Id);
        }

        [Route("~/" + CollectionAPI.API_PREFIX + CollectionAPI.COLLECTION_PREFIX + Route.EVENT_CONTROLLER + "/{id:int}/specimen")]
        [HttpGet]
        public async Task<IHttpActionResult> SpecimenForEvent(int id)
        {
            var all = await Store.GetQueryableAsync();

            var specimenForEvent = from x in all
                                   where x.EventID == id
                                   orderby x.Id
                                   select x;

            return PagedAndMapped<Collection.Specimen, Specimen>(specimenForEvent);
        }
    }
}