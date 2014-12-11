namespace DiversityService.API.Test
{
    using DiversityService.API.Controllers;
    using DiversityService.API.Model;
    using DiversityService.API.Results;
    using DiversityService.API.Services;
    using Moq;
    using Ninject;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Web.Http.Results;
    using Xunit;

    public class SeriesControllerTest : ControllerTestBase<SeriesController>
    {
        private readonly Mock<IStore<Collection.EventSeries, int>> MockSeriesStore;

        public SeriesControllerTest()
        {
            MockSeriesStore = Kernel.GetMock<IStore<Collection.EventSeries, int>>();
            InitController();
        }

        [Fact]
        public async Task Returns_Series_with_matching_Id_on_GET()
        {
            // Arrange
            var collSeries = new Collection.EventSeries()
            {
                Id = 1234
            };
            this.MockSeriesStore
                .Setup(x => x.GetByIDAsync(collSeries.Id))
                .Returns(Task.FromResult(collSeries));

            // Act
            var result = await Controller.Get(collSeries.Id) as OkNegotiatedContentResult<EventSeries>;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(collSeries.Id, result.Content.Id);
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
                new Collection.EventSeries() { Code = matchCode, Id = 0},
                new Collection.EventSeries() { Code = nonmatchCode, Id = 4},
                new Collection.EventSeries() { Code = matchCode, Id = 2},
                new Collection.EventSeries() { Code = nonmatchCode, Id = 1}
            }.AsQueryable();

            this.MockSeriesStore
                // Called with defaults
                .Setup(x => x.GetQueryableAsync())
                .Returns(Task.FromResult(fakeCollSeries));

            // Act
            var result = await Controller.Get(query) as PagingResult<EventSeries>;
            var content = result.Content.ToList();

            // Assert
            Assert.True(result.Content.All(x => x.Code == matchCode), "Query returned a non-matching result");
            // result is ordered by id
            Assert.Equal(content.OrderBy(x => x.Id), content);
            Assert.Equal(2, result.Content.Count());
        }

        [Fact]
        public async Task Returns_404_for_nonexistent_Series_on_GET()
        {
            // Arrange
            int invalidId = 12345;
            MockSeriesStore
                .Setup(x => x.GetByIDAsync(invalidId))
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
                .Setup(x => x.GetQueryableAsync())
                .Returns(Task.FromResult(fakeCollSeries));

            // Act
            var result = await Controller.Get() as PagingResult<EventSeries>;
            var content = result.Content.ToList();

            // Assert
            Assert.Equal(fakeSeries.Count(), result.Content.Count());
            // result is ordered by id
            Assert.Equal(content.OrderBy(x => x.Id), content);
            Assert.DoesNotContain(null, result.Content);
        }

        [Fact]
        public async Task Inserts_A_New_Series_on_POST()
        {
            // Arrange
            var id = TestHelper.RandomInt();
            var series = new EventSeriesBindingModel() { TransactionGuid = Guid.NewGuid() };
            var collSeries = new Collection.EventSeries() { RowGUID = series.TransactionGuid };

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
                .SetupWithFakeData<IStore<Collection.EventSeries, int>, Collection.EventSeries, int>(collSeries.AsQueryable());

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