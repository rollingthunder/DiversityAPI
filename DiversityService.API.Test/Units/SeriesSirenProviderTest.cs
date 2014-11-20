namespace DiversityService.API.Test
{
    using DiversityService.API.Filters;
    using DiversityService.API.Model;
    using System.Linq;
    using Xunit;

    public class SeriesSirenProviderTest
    {
        private EventSeries Series;

        private ISirenProvider Provider;

        public SeriesSirenProviderTest()
        {
            Series = new EventSeries();
            Provider = new SeriesSirenProvider(null);
        }

        [Fact]
        public void Accepts_Series()
        {
            // Arrange

            // Act
            var acceptsSeries = Provider.CanTranslate(typeof(EventSeries));

            // Assert
            Assert.True(acceptsSeries);
        }

        [Fact]
        public void Adds_Self_Link()
        {
            // Arrange

            // Act
            var siren = Provider.Translate(Series);

            // Assert
            var selfLink = siren.Links.Single(x => x.Rel.Contains("self"));
            Assert.Equal(selfLink.Href, "/series/0");
        }
    }
}