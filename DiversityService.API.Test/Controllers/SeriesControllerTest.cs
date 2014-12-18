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

    public class SeriesControllerTest : FieldDataControllerTest<SeriesController, Collection.EventSeries, EventSeries, EventSeriesBindingModel>
    {
        public SeriesControllerTest()
        {
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

            this.MockStore
                .SetupWithFakeData<IStore<Collection.EventSeries, int>, Collection.EventSeries, int>(fakeCollSeries);

            // Act
            var result = await Controller.Get(query) as PagingResult<EventSeries>;
            var content = result.Content.ToList();

            // Assert
            Assert.True(result.Content.All(x => x.Code == matchCode), "Query returned a non-matching result");
            // result is ordered by id
            Assert.Equal(content.OrderBy(x => x.Id), content);
            Assert.Equal(2, result.Content.Count());
        }
    }
}