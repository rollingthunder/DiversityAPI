namespace DiversityService.API.Test
{
    using DiversityService.API.Controllers;
    using DiversityService.API.Model;
    using DiversityService.API.Results;
    using DiversityService.API.Services;
    using Moq;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Web.Http;
    using System.Web.Http.Results;
    using Xunit;

    public class FieldDataControllerTest<TController, TEntity, TDto, TUpload> : ControllerTestBase<TController>
        where TController : ApiController, IFieldDataController<TUpload>
        where TEntity : class, IIdentifiable, IGuidIdentifiable, new()
        where TDto : class, IIdentifiable, new()
        where TUpload : IGuidIdentifiable, new()
    {
        protected readonly Mock<IStore<TEntity, int>> MockStore;

        public FieldDataControllerTest()
        {
            MockStore = Kernel.GetMock<IStore<TEntity, int>>();
            InitController();
        }

        [Fact]
        public async Task Returns_Entity_with_matching_Id_on_GET()
        {
            // Arrange
            var entity = new TEntity()
            {
                Id = 1234
            };
            this.MockStore
                .Setup(x => x.GetByIDAsync(entity.Id))
                .Returns(Task.FromResult(entity));

            // Act
            var result = await Controller.Get(entity.Id) as OkNegotiatedContentResult<TDto>;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(entity.Id, result.Content.Id);
        }

        [Fact]
        public async Task Returns_404_for_nonexistent_Entity_on_GET()
        {
            // Arrange
            int invalidId = 12345;
            MockStore
                .Setup(x => x.GetByIDAsync(invalidId))
                .Returns(Task.FromResult<TEntity>(null)); // Simulate no match

            // Act
            var result = await Controller.Get(invalidId) as NotFoundResult;

            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public async Task Returns_all_for_unqualified_GET()
        {
            // Arrange
            var fakeEntities =
                new[] {
                    new TEntity() { Id = TestHelper.RandomInt() },
                    new TEntity() { Id = TestHelper.RandomInt() },
                    new TEntity() { Id = TestHelper.RandomInt() },
                    new TEntity() { Id = TestHelper.RandomInt() },
                    new TEntity() { Id = TestHelper.RandomInt() },
                };
            this.MockStore
                .SetupWithFakeData<IStore<TEntity, int>, TEntity, int>(fakeEntities.AsQueryable());

            // Act
            var result = await Controller.Get() as PagingResult<TDto>;
            var content = result.Content.ToList();

            // Assert
            Assert.Equal(fakeEntities.Count(), result.Content.Count());
            // result is ordered by id
            Assert.Equal(content.OrderBy(x => x.Id), content);
            Assert.DoesNotContain(null, result.Content);
        }

        [Fact]
        public async Task Inserts_A_New_Entity_on_POST()
        {
            // Arrange
            var id = TestHelper.RandomInt();
            var series = new TUpload() { TransactionGuid = Guid.NewGuid() };

            MockStore
                .SetupInsert(x => x.Id = id);

            // Act
            var result = await Controller.Post(series) as CreatedAtRouteNegotiatedContentResult<int>;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(id, result.Content);
        }

        [Fact]
        public async Task Returns_redirect_for_existing_TransactionGUID_on_POST()
        {
            // Arrange
            var id = TestHelper.RandomInt();
            var series = new TUpload() { TransactionGuid = Guid.NewGuid() };
            var entities = new[] { new TEntity() { Id = id, TransactionGuid = series.TransactionGuid } };

            MockStore
                .SetupWithFakeData(entities.AsQueryable());

            // Act
            var result = await Controller.Post(series) as SeeOtherAtRouteResult;

            // Assert
            MockStore.Verify(x => x.InsertAsync(It.IsAny<TEntity>()), Times.Never());
            Assert.NotNull(result);
            Assert.Equal(Route.DEFAULT_API, result.RouteName);
            Assert.Equal(id, (int)result.RouteValues[Route.PARAM_ID]);
        }
    }
}