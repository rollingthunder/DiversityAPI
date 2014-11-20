namespace DiversityService.API.Test.Units
{
    using DiversityService.API.Filters;
    using DiversityService.API.Services;
    using Ninject;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Xunit;

    public class RequireCollectionAttributeTest : FilterTestBase<RequireCollectionAttribute>
    {
        public RequireCollectionAttributeTest()
        {
            InitializeActionContext();

            Filter = Kernel.Get<RequireCollectionAttribute>();
        }

        [Fact]
        public async Task Rejects_Without_Context()
        {
            // Arrange
            // Act
            await InvokeFilter();

            // Assert
            Assert.False(ActionCalled());
            Assert.False(ActionContext.Response.IsSuccessStatusCode);
        }

        [Fact]
        public async Task Accepts_With_Context()
        {
            // Arrange
            Request
                .SetCollectionContext(Kernel.Get<IContext>());

            // Act
            await InvokeFilter();

            // Assert
            Assert.True(ActionCalled());
        }
    }
}