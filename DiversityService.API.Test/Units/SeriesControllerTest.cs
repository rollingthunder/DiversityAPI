namespace DiversityService.API.Test
{
    using DiversityService.API.Controllers;
    using DiversityService.API.Model;
    using DiversityService.API.Results;
    using DiversityService.API.Services;
    using Moq;
    using Ninject;
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Web.Http.Results;
    using Xunit;

    public class SeriesControllerTest
    {
        private readonly TestKernel Kernel = new TestKernel();
        private readonly Mock<ISeriesStore> MockSeriesStore;
        private readonly Mock<IMappingService> MockMappingService;
        private readonly SeriesController Controller;

        public SeriesControllerTest()
        {
            MockSeriesStore = Kernel.GetMock<ISeriesStore>();
            MockMappingService = Kernel.GetMock<IMappingService>();
            Controller = Kernel.Get<SeriesController>();
        }

        [Fact]
        public async Task Returns_Series_with_matching_Id_on_GET()
        {
            // Arrange
            var collSeries = new Collection.EventSeries()
            {
                Id = 1234
            };
            var series = new EventSeries() { Id = collSeries.Id };
            this.MockSeriesStore
                .Setup(x => x.FindAsync(collSeries.Id))
                .Returns(Task.FromResult(collSeries));
            this.MockMappingService
                .Setup(x => x.Map<EventSeries>(collSeries))
                .Returns(series);

            // Act
            var result = await Controller.Get(collSeries.Id) as OkNegotiatedContentResult<EventSeries>;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(series, result.Content);
        }

        [Fact]
        public async Task Returns_Series_filtered_by_code_on_query()
        {
            // Arrange
            var matchCode = "match";
            var nonmatchCode = "non";
            var query = "atc";

            var fakeCollSeries = new[]
            {
                new Collection.EventSeries() { Code = matchCode },
                new Collection.EventSeries() { Code = nonmatchCode},
                new Collection.EventSeries() { Code = matchCode},
                new Collection.EventSeries() { Code = nonmatchCode}
            }.AsQueryable();

            this.MockSeriesStore
                .Setup(x => x.FindAsync())
                .Returns(Task.FromResult(fakeCollSeries));
            this.MockMappingService
                .Setup(x => x.Project<Collection.EventSeries, EventSeries>(It.IsAny<IQueryable<Collection.EventSeries>>()))
                .Returns((IQueryable<Collection.EventSeries> x) =>
                {
                    return from cs in x
                           select new EventSeries() { Code = cs.Code };
                });

            // Act
            var result = await Controller.Get(query);

            // Assert
            Assert.True(result.All(x => x.Code == matchCode), "Query returned a non-matching result");
            Assert.Equal(2, result.Count());
        }

        [Fact]
        public async Task Returns_404_for_nonexistent_Series_on_GET()
        {
            // Arrange
            int invalidId = 12345;
            MockSeriesStore
                .Setup(x => x.FindAsync(invalidId))
                .Returns(Task.FromResult<Collection.EventSeries>(null)); // Simulate no match

            // Act
            var result = await Controller.Get(invalidId) as NotFoundResult;

            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public async Task Returns_all_series_for_unqualified_GET()
        {
            // Arrange
            var fakeCollSeries = (new[] { new Collection.EventSeries(), new Collection.EventSeries() }).AsQueryable();
            var fakeSeries = (new[] { new EventSeries(), new EventSeries() }).AsQueryable();
            var series = new EventSeries();
            this.MockSeriesStore
                .Setup(x => x.FindAsync())
                .Returns(Task.FromResult(fakeCollSeries));
            this.MockMappingService
                .Setup(x => x.Project<Collection.EventSeries, EventSeries>(fakeCollSeries))
                .Returns(fakeSeries);

            // Act
            var result = await Controller.Get();

            // Assert
            Assert.Equal(fakeSeries.Count(), result.Count());
            Assert.DoesNotContain(result, null);
        }

        [Fact]
        public async Task Inserts_A_New_Series_on_POST()
        {
            // Arrange
            var id = TestHelper.RandomInt();
            var series = new EventSeriesBindingModel() { TransactionGuid = Guid.NewGuid() };
            var collSeries = new Collection.EventSeries() { RowGUID = series.TransactionGuid };

            MockMappingService
                .Setup(x => x.Map<Collection.EventSeries>(series))
                .Returns(collSeries);

            MockSeriesStore.Setup(x => x.InsertAsync(collSeries))
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
            var series = new EventSeriesBindingModel() { TransactionGuid = Guid.NewGuid() };
            var collSeries = new[] { new Collection.EventSeries() { Id = id, RowGUID = series.TransactionGuid } };

            MockSeriesStore
                .Setup(x => x.FindAsync())
                .Returns(Task.FromResult(collSeries.AsQueryable()));

            // Act
            var result = await Controller.Post(series) as SeeOtherAtRouteResult;

            // Assert
            MockSeriesStore.Verify(x => x.InsertAsync(It.IsAny<Collection.EventSeries>()), Times.Never());
            Assert.NotNull(result);
            Assert.Equal(Route.DEFAULT_API, result.RouteName);
            Assert.Equal(id, (int)result.RouteValues[Route.PARAM_ID]);
        }
    }
}