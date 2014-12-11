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

    public class EventControllerTest : ControllerTestBase<EventController>
    {
        private readonly Mock<IStore<Collection.Event, int>> MockEventStore;

        public EventControllerTest()
        {
            MockEventStore = Kernel.GetMock<IStore<Collection.Event, int>>();

            InitController();
        }

        [Fact]
        public async Task Returns_Event_with_matching_Id_on_GET()
        {
            // Arrange
            var colEvent = new Collection.Event()
            {
                Id = 1234
            };
            this.MockEventStore
                .Setup(x => x.GetByIDAsync(colEvent.Id))
                .Returns(Task.FromResult(colEvent));

            // Act
            var result = await Controller.Get(colEvent.Id) as OkNegotiatedContentResult<Event>;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(colEvent.Id, result.Content.Id);
        }

        [Fact]
        public async Task Returns_404_for_nonexistent_Event_on_GET()
        {
            // Arrange
            int invalidId = 12345;
            MockEventStore
                .Setup(x => x.GetByIDAsync(invalidId))
                .Returns(Task.FromResult<Collection.Event>(null)); // Simulate no match

            // Act
            var result = await Controller.Get(invalidId) as NotFoundResult;

            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public async Task Returns_all_events_for_unqualified_GET()
        {
            // Arrange
            var fakeCollSeries = new[] {
                new Collection.Event() { Id = 4},
                new Collection.Event() { Id = 3}
            };
            var series = new Event();
            this.MockEventStore
                .Setup(x => x.GetQueryableAsync())
                .Returns(Task.FromResult(fakeCollSeries.AsQueryable()));

            // Act
            var result = await Controller.Get() as PagingResult<Event>;
            var content = result.Content.ToList();

            // Assert
            Assert.Equal(fakeCollSeries.Count(), content.Count());
            // returns ordered by id
            Assert.Equal(content.OrderBy(x => x.Id), content);
            Assert.DoesNotContain(null, content);
        }

        [Fact]
        public async Task Inserts_A_New_Event_on_POST()
        {
            // Arrange
            var id = TestHelper.RandomInt();
            var series = new EventBindingModel() { TransactionGuid = Guid.NewGuid() };
            var collSeries = new Collection.Event() { RowGUID = series.TransactionGuid };

            MockEventStore.Setup(x => x.InsertAsync(collSeries))
                .Callback(() => collSeries.Id = id)
                .Returns(Task.FromResult(false));

            // Act
            var result = await Controller.Post(series) as CreatedAtRouteNegotiatedContentResult<int>;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(collSeries.Id, result.Content);
        }

        [Fact]
        public async Task Returns_redirect_for_existing_TransactionGUID_on_POST()
        {
            // Arrange
            var id = TestHelper.RandomInt();
            var series = new EventBindingModel() { TransactionGuid = Guid.NewGuid() };
            var data = new[] { new Collection.Event() { Id = id, RowGUID = series.TransactionGuid } };

            MockEventStore
                .SetupWithFakeData<IStore<Collection.Event, int>, Collection.Event, int>(data.AsQueryable());

            // Act
            var result = await Controller.Post(series) as SeeOtherAtRouteResult;

            // Assert
            MockEventStore.Verify(x => x.InsertAsync(It.IsAny<Collection.Event>()), Times.Never());
            Assert.NotNull(result);
            Assert.Equal(Route.DEFAULT_API, result.RouteName);
            Assert.Equal(id, (int)result.RouteValues[Route.PARAM_ID]);
        }

        [Fact]
        public async Task Returns_Events_for_Series()
        {
            // Arrange
            var seriesId = TestHelper.RandomInt();
            var collEvents = new[]
            {
                new Collection.Event() { SeriesID = seriesId },
                new Collection.Event() { SeriesID = TestHelper.RandomInt() },
                new Collection.Event() { SeriesID = seriesId },
                new Collection.Event() { SeriesID = TestHelper.RandomInt() },
                new Collection.Event() { SeriesID = seriesId },
                new Collection.Event() { SeriesID = TestHelper.RandomInt() },
                new Collection.Event() { SeriesID = seriesId },
                new Collection.Event() { SeriesID = TestHelper.RandomInt() },
            };
            MockEventStore
                .Setup(x => x.GetQueryableAsync())
                .Returns(Task.FromResult(collEvents.AsQueryable()));

            // Act
            var result = await Controller.EventsForSeries(seriesId) as PagingResult<Event>;
            var content = result.Content.ToList();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(4, result.Content.Count());
            // returns ordered by id
            Assert.Equal(content.OrderBy(x => x.Id), content);
            Assert.True(content.All(x => x.SeriesId == seriesId), "Result Contained Event not associated with the given series");
        }
    }
}