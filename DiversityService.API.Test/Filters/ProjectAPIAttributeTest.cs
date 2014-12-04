namespace DiversityService.API.Test
{
    using DiversityService.API.Filters;
    using DiversityService.API.Services;
    using Ninject;
    using System.Net;
    using System.Threading.Tasks;
    using Xunit;

    public class ProjectAPIAttributeTest : FilterTestBase<ProjectAPIAttribute>
    {
        public ProjectAPIAttributeTest()
        {
            InitializeActionContext();

            Filter = new ProjectAPIAttribute("");
        }

        [Fact]
        public async Task Rejects_Without_Context()
        {
            // Arrange
            // Act
            await InvokeFilter();

            // Assert
            Assert.False(ActionCalled());
            Assert.Equal(HttpStatusCode.Forbidden, ActionContext.Response.StatusCode);
        }

        [Fact]
        public async Task Rejects_Without_Id()
        {
            // Arrange
            Request
                .SetCollectionContext(Kernel.Get<IContext>());

            // Act
            await InvokeFilter();

            // Assert
            Assert.False(ActionCalled());
            Assert.Equal(HttpStatusCode.Forbidden, ActionContext.Response.StatusCode);
        }

        [Fact]
        public async Task Accepts_With_Context_And_Id()
        {
            // Arrange
            var ctx = Kernel.GetMock<IContext>();

            ctx
                .SetupGet(x => x.ProjectId)
                .Returns(0);

            Request
                .SetCollectionContext(Kernel.Get<IContext>());

            // Act
            await InvokeFilter();

            // Assert
            Assert.True(ActionCalled());
        }
    }
}