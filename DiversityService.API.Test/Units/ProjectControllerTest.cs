namespace DiversityService.API.Test
{
    using DiversityService.API.Controllers;
    using DiversityService.API.Filters;
    using System;
    using System.Linq;
    using Xunit;

    public class ProjectControllerTest
    {
        [Fact]
        public void Requires_a_Collection()
        {
            // Arrange

            // Act
            var hasCollectionFilter = (typeof(ProjectController).GetCustomAttributes(typeof(RequireCollectionAttribute), true)).Any();

            // Assert
            Assert.True(hasCollectionFilter);
        }
    }
}