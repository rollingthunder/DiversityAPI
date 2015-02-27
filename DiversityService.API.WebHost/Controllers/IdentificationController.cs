namespace DiversityService.API.Controllers
{
    using AutoMapper;
    using DiversityService.API.Filters;
    using DiversityService.API.Model;
    using DiversityService.API.Services;
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Web.Http;

    [CollectionAPI(Route.SPECIMEN_CONTROLLER + "/{sid:int}/" + Route.IDENTIFICATION_CONTROLLER)]
    public class IdentificationController : DiversityController
    {
        private const string DEFAULT_NOTES = "Created by DiversityMobile";
        private const string IDENTIFICATION_CATEGORY_ACTUAL = "actual";

        private IStore<Collection.IdentificationUnit, Collection.IdentificationUnitKey> IUStore
        {
            get
            {
                return Request.GetCollectionContext().IdentificationUnits;
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

        private ITransaction BeginTransaction()
        {
            return Request.GetCollectionContext().BeginTransaction();
        }

        private AgentInfo AgentInfo
        {
            get
            {
                return Request.GetAgentInfo();
            }
        }

        public IdentificationController(
            IMappingEngine mapper
            )
            : base(mapper)
        {
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

        [Route("{id}", Name = Route.IDENTIFICATION_BYID)]
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
            var existing = await RedirectToExisting(IUStore, value.TransactionGuid, Route.IDENTIFICATION_BYID);

            if (existing != null)
            {
                return existing;
            }

            Identification dto = null;

            using (var transaction = BeginTransaction())
            {
                // Identification Info is stored in three distinct tables

                // IdentificationUnit

                var newIU = Mapper.Map<Collection.IdentificationUnit>(value);

                newIU.SpecimenId = sid;
                newIU.LastIdentificationCache = value.Name;

                await IUStore.InsertAsync(newIU);

                dto = Mapper.Map<Collection.IdentificationUnit, Identification>(newIU, value);

                // Identification

                var newID = Mapper.Map<Collection.Identification>(dto); // From dto to get the db generated Id

                newID.IdentificationCategory = IDENTIFICATION_CATEGORY_ACTUAL;
                newID.Notes = DEFAULT_NOTES;
                newID.ResponsibleName = AgentInfo.Name;
                newID.ResponsibleAgentURI = AgentInfo.Uri;

                await IDStore.InsertAsync(newID);

                // IdentificationUnitGeoAnalysis

                var newIUGAN = Mapper.Map<Collection.IdentificationUnitGeoAnalysis>(dto);

                newIUGAN.Geography = dto.Localization.ToGeography();
                newIUGAN.Notes = DEFAULT_NOTES;

                newIUGAN.ResponsibleName = AgentInfo.Name;
                newIUGAN.ResponsibleAgentURI = AgentInfo.Uri;

                await IUGANStore.InsertAsync(newIUGAN);

                transaction.Commit();
            }

            return CreatedAtRoute(Route.IDENTIFICATION_BYID, Route.GetById(dto), dto);
        }
    }
}