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

    [CollectionAPI(Route.SpecimenController + "/{sid:int}/" + Route.IdentificationController)]
    public class IdentificationController : DiversityController
    {
        private const string DefaultNotes = "Created by DiversityMobile";
        private const string IdentificationCategoryActual = "actual";

        public IdentificationController(
            IMappingEngine mapper)
            : base(mapper)
        {
        }

        private AgentInfo AgentInfo
        {
            get
            {
                return Request.GetAgentInfo();
            }
        }

        private IStore<Collection.Identification, Collection.IdentificationKey> IDStore
        {
            get
            {
                return Request.GetCollectionContext().Identifications;
            }
        }

        private IStore<Collection.IdentificationUnitGeoAnalysis, Collection.IdentificationGeoKey> IUGANStore
        {
            get
            {
                return Request.GetCollectionContext().IdentificationGeoAnalyses;
            }
        }

        private IStore<Collection.IdentificationUnit, Collection.IdentificationUnitKey> IUStore
        {
            get
            {
                return Request.GetCollectionContext().IdentificationUnits;
            }
        }

        [Route]
        public async Task<IHttpActionResult> Get(int sid)
        {
            var all = await IUStore.GetQueryableAsync();

            var query = all
                .Where(x => x.SpecimenId == sid)
                .OrderBy(x => x.Id);

            return PagedAndMapped<Collection.IdentificationUnit, Identification>(query);
        }

        [Route("{id}", Name = Route.IdentificationById)]
        public async Task<IHttpActionResult> Get(int sid, int id)
        {
            var key = new Collection.IdentificationUnitKey() { SpecimenId = sid, IdentificationUnitId = id };
            var x = await IUStore.GetByIDAsync(key);

            if (x == null)
            {
                return NotFound();
            }

            var dto = Mapper.Map<Identification>(x);

            return Ok(dto);
        }

        [Route]
        public async Task<IHttpActionResult> Post(int sid, IdentificationBindingModel value)
        {
            var existing = await RedirectToExisting(IUStore, value.TransactionGuid, Route.IdentificationById);

            if (existing != null)
            {
                return existing;
            }

            Identification dto = value;

            using (var transaction = BeginTransaction())
            {
                // Identification Info is stored in three distinct tables 

                // IdentificationUnit 
                var newIU = Mapper.Map<Collection.IdentificationUnit>(value);

                newIU.SpecimenId = sid;
                newIU.LastIdentificationCache = value.Name;

                await IUStore.InsertAsync(newIU);

                dto.Id = newIU.Id;

                // Identification 
                var newID = Mapper.Map<Collection.Identification>(dto);

                newID.IdentificationCategory = IdentificationCategoryActual;
                newID.Notes = DefaultNotes;
                newID.ResponsibleName = AgentInfo.Name;
                newID.ResponsibleAgentURI = AgentInfo.Uri;

                await IDStore.InsertAsync(newID);

                // IdentificationUnitGeoAnalysis 
                var newIUGAN = Mapper.Map<Collection.IdentificationUnitGeoAnalysis>(dto);

                newIUGAN.Geography = dto.Localization.ToGeography();
                newIUGAN.Notes = DefaultNotes;
                newIUGAN.ResponsibleName = AgentInfo.Name;
                newIUGAN.ResponsibleAgentURI = AgentInfo.Uri;

                await IUGANStore.InsertAsync(newIUGAN);

                transaction.Commit();
            }

            return CreatedAtRoute(Route.IdentificationById, Route.GetById(dto), dto);
        }

        private ITransaction BeginTransaction()
        {
            return Request.GetCollectionContext().BeginTransaction();
        }
    }
}