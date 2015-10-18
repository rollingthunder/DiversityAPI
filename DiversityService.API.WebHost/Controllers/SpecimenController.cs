namespace DiversityService.API.Controllers
{
    using System.Linq;
    using System.Threading.Tasks;
    using System.Web.Http;
    using AutoMapper;
    using DiversityService.API.Filters;
    using DiversityService.API.Model;
    using DiversityService.API.Services;
    using Collection = DiversityService.DB.Collection;

    [CollectionAPI(Route.SpecimenController)]
    public class SpecimenController : DiversityController, IFieldDataController<SpecimenBindingModel>
    {
        public SpecimenController(
         IMappingEngine mapper)
         : base(mapper)
        {
        }

        private IStore<Collection.Specimen, int> Store
        {
            get
            {
                return Request.GetCollectionContext().Specimen;
            }
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
            var existing = await RedirectToExisting(Store, value.TransactionGuid, Route.SpecimenById);

            if (existing != null)
            {
                return existing;
            }

            var newEntity = Mapper.Map<Collection.Specimen>(value);

            await Store.InsertAsync(newEntity);

            var dto = Mapper.Map<Specimen>(newEntity);

            return CreatedAtRoute(Route.SeriesById, Route.GetById(newEntity), dto);
        }

        [Route("~/" + CollectionAPI.ApiPrefix + CollectionAPI.CollectionPrefix + Route.EventController + "/{id:int}/specimen")]
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