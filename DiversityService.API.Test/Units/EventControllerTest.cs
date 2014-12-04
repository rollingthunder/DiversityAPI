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
        private readonly Mock<IMappingService> MockMappingService;

        public EventControllerTest()
        {
            MockEventStore = Kernel.GetMock<IStore<Collection.Event, int>>();
            MockMappingService = Kernel.GetMock<IMappingService>();

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
            var ev = new Event() { Id = colEvent.Id };
            this.MockEventStore
                .Setup(x => x.GetByIDAsync(colEvent.Id))
                .Returns(Task.FromResult(colEvent));
            this.MockMappingService
                .Setup(x => x.Map<Event>(colEvent))
                .Returns(ev);

            // Act
            var result = await Controller.Get(colEvent.Id) as OkNegotiatedContentResult<Event>;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(ev, result.Content);
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
            var fakeCollSeries = (new[] { new Collection.Event(), new Collection.Event() }).AsQueryable();
            var fakeSeries = (new[] { new Event(), new Event() }).AsQueryable();
            var series = new Event();
            this.MockEventStore
                .Setup(x => x.GetQueryableAsync())
                .Returns(Task.FromResult(fakeCollSeries));
            this.MockMappingService
                .Setup(x => x.Project<Collection.Event, Event>(fakeCollSeries))
                .Returns(fakeSeries);

            // Act
            var result = await Controller.Get();

            // Assert
            Assert.Equal(fakeSeries.Count(), result.Count());
            Assert.DoesNotContain(result, null);
        }

        [Fact]
        public async Task Inserts_A_New_Event_on_POST()
        {
            // Arrange
            var id = TestHelper.RandomInt();
            var series = new EventBindingModel() { TransactionGuid = Guid.NewGuid() };
            var collSeries = new Collection.Event() { RowGUID = series.TransactionGuid };

            MockMappingService
                .Setup(x => x.Map<Collection.Event>(series))
                .Returns(collSeries);

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
            MockMappingService
                .Setup(x => x.Project<Collection.Event, Event>(It.IsAny<IQueryable<Collection.Event>>()))
                .Returns((IQueryable<Collection.Event> evs) => from ev in evs
                                                               select new Event() { SeriesId = ev.SeriesID });

            // Act
            var result = await Controller.EventsForSeries(seriesId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(4, result.Count());
            Assert.True(result.All(x => x.SeriesId == seriesId), "Result Contained Event not associated with the given series");
        }
    }
}