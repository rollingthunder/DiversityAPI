namespace DiversityService.API.Test.Units
{
    using DiversityService.API.Controllers;
    using DiversityService.API.Model;
    using DiversityService.API.Results;
    using DiversityService.API.Services;
    using Moq;
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Web.Http.Results;
    using Xunit;
    using Collection = DiversityService.DB.Collection;

    public class SpecimenControllerTest : CRUDControllerTest<SpecimenController, Collection.Specimen, Specimen, SpecimenBindingModel>
    {
        public SpecimenControllerTest()
        {
        }

        [Fact]
        public async Task Returns_Specimen_for_Event()
        {
            // Arrange
            var eventId = TestHelper.RandomInt();
            var collEvents = new[]
            {
                new Collection.Specimen() { EventID = eventId, Id = TestHelper.RandomInt() },
                new Collection.Specimen() { EventID = TestHelper.RandomInt(), Id = TestHelper.RandomInt() },
                new Collection.Specimen() { EventID = eventId, Id = TestHelper.RandomInt() },
                new Collection.Specimen() { EventID = TestHelper.RandomInt(), Id = TestHelper.RandomInt() },
                new Collection.Specimen() { EventID = eventId, Id = TestHelper.RandomInt() },
                new Collection.Specimen() { EventID = TestHelper.RandomInt(), Id = TestHelper.RandomInt() },
                new Collection.Specimen() { EventID = eventId, Id = TestHelper.RandomInt() },
                new Collection.Specimen() { EventID = TestHelper.RandomInt(), Id = TestHelper.RandomInt() },
            };
            MockStore
                .SetupWithFakeData(collEvents.AsQueryable());

            // Act
            var result = await Controller.SpecimenForEvent(eventId) as IQueryResult<Specimen>;
            var content = result.Query.ToList();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(4, result.Query.Count());
            // returns ordered by id
            Assert.Equal(content.OrderBy(x => x.Id), content);
            Assert.True(content.All(x => x.EventId == eventId), "Result Contained Specimen not associated with the given parent");
        }
    }
}