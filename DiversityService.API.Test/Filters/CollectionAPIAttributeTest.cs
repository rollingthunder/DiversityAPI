namespace DiversityService.API.Test
{
    using DiversityService.API.Filters;
    using DiversityService.API.Services;
    using Ninject;
    using System.Net;
    using System.Threading.Tasks;
    using Xunit;

    public class RequireCollectionAttributeTest : FilterTestBase<CollectionAPIAttribute>
    {
        public RequireCollectionAttributeTest()
        {
            InitializeActionContext();

            Filter = new CollectionAPIAttribute("");
        }

        [Fact]
        public async Task Rejects_Without_Context()
        {
            // Arrange
            // Act
            await ActionExecuting();

            // Assert
            Assert.False(ActionCalled());
            Assert.Equal(HttpStatusCode.Forbidden, ActionContext.Response.StatusCode);
        }

        [Fact]
        public async Task Accepts_With_Context()
        {
            // Arrange
            Request
                .SetCollectionContext(Kernel.Get<IContext>());

            // Act
            await ActionExecuting();

            // Assert
            Assert.True(ActionCalled());
        }
    }
}