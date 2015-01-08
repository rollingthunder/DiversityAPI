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

    public class EventControllerTest : FieldDataControllerTest<EventController, Collection.Event, Event, EventBindingModel>
    {
        public EventControllerTest()
        {
        }

        [Fact]
        public async Task Returns_Events_for_Series()
        {
            // Arrange
            var seriesId = TestHelper.RandomInt();
            var collEvents = new[]
            {
                new Collection.Event() { SeriesID = seriesId, Id = TestHelper.RandomInt() },
                new Collection.Event() { SeriesID = TestHelper.RandomInt(), Id = TestHelper.RandomInt() },
                new Collection.Event() { SeriesID = seriesId, Id = TestHelper.RandomInt() },
                new Collection.Event() { SeriesID = TestHelper.RandomInt(), Id = TestHelper.RandomInt() },
                new Collection.Event() { SeriesID = seriesId, Id = TestHelper.RandomInt() },
                new Collection.Event() { SeriesID = TestHelper.RandomInt(), Id = TestHelper.RandomInt() },
                new Collection.Event() { SeriesID = seriesId, Id = TestHelper.RandomInt() },
                new Collection.Event() { SeriesID = TestHelper.RandomInt(), Id = TestHelper.RandomInt() },
            };
            MockStore
                .SetupWithFakeData(collEvents.AsQueryable());

            // Act
            var result = await Controller.EventsForSeries(seriesId) as IQueryResult<Event>;
            var content = result.Query.ToList();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(4, result.Query.Count());
            // returns ordered by id
            Assert.Equal(content.OrderBy(x => x.Id), content);
            Assert.True(content.All(x => x.SeriesId == seriesId), "Result Contained Event not associated with the given series");
        }

        [Fact]
        public async Task Returns_Events_for_NullSeries()
        {
            // Arrange
            var seriesId = null as int?;
            var collEvents = new[]
            {
                new Collection.Event() { SeriesID = seriesId, Id = TestHelper.RandomInt() },
                new Collection.Event() { SeriesID = TestHelper.RandomInt(), Id = TestHelper.RandomInt() },
                new Collection.Event() { SeriesID = seriesId, Id = TestHelper.RandomInt() },
                new Collection.Event() { SeriesID = TestHelper.RandomInt(), Id = TestHelper.RandomInt() },
                new Collection.Event() { SeriesID = seriesId, Id = TestHelper.RandomInt() },
                new Collection.Event() { SeriesID = TestHelper.RandomInt(), Id = TestHelper.RandomInt() },
                new Collection.Event() { SeriesID = seriesId, Id = TestHelper.RandomInt() },
                new Collection.Event() { SeriesID = TestHelper.RandomInt(), Id = TestHelper.RandomInt() },
            };
            MockStore
                .SetupWithFakeData(collEvents.AsQueryable());

            // Act
            var result = await Controller.EventsForNullSeries() as IQueryResult<Event>;
            var content = result.Query.ToList();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(4, result.Query.Count());
            // returns ordered by id
            Assert.Equal(content.OrderBy(x => x.Id), content);
            Assert.True(content.All(x => x.SeriesId == seriesId), "Result Contained Event not associated with the given series");
        }
    }
}