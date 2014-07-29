using AutoMapper;
using DiversityService.API.Model;
using DiversityService.API.WebHost;
using DiversityService.API.WebHost.Controllers;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Routing;
using Xunit;

namespace DiversityService.API.Test
{
    public class SeriesControllerTest
    {
        private static Random Rnd = new Random();
        public static Collection.EventSeries RandomSeries()
        {
            return new Collection.EventSeries()
            {
                Id = Rnd.Next()
            };
        }

        private readonly Mock<IRepository<Collection.EventSeries>> Repository;
        private readonly Mock<IUnitOfWork> UOW;
        private readonly Mock<UrlHelper> Urls;
        private readonly IMappingEngine Mapping;
        private SeriesController Target;

        public SeriesControllerTest()
        {
            this.Repository = new Mock<IRepository<Collection.EventSeries>>();            
            this.UOW = new Mock<IUnitOfWork>();
            this.Urls = new Mock<UrlHelper>();            

            this.Repository.Setup(x => x.Transaction).Returns(this.UOW.Object);

            Mapper.Initialize(MapperConfig.Configure);
            this.Mapping = Mapper.Engine;
        }

        private void SetupLink(string Link) 
        {
            this.Urls
               .Setup(x => x.Link(It.IsAny<string>(), It.IsAny<IDictionary<string, object>>()))
               .Returns(Link);
        }

        private void VerifyLink(Expression<Func<IDictionary<string, object>, bool>> predicate) 
        {
            this.Urls
                .Verify(x => x.Link(It.IsAny<string>(), It.Is<IDictionary<string, object>>(predicate)));
        }

        private void CreateTarget()
        {
            var repoFactory = new Mock<IRepositoryFactory>();
            repoFactory.Setup(x => x.Get<Collection.EventSeries>())
                .Returns(Repository.Object);

            this.Target = new SeriesController(repoFactory.Object, this.Mapping);
            this.Target.Request = new HttpRequestMessage();
            this.Target.Configuration = new HttpConfiguration();
            this.Target.Url = this.Urls.Object;
        }        

        [Fact]
        public async Task Returns_A_Series_By_Id()
        {
            // Arrange        
            int id = 12345;
            var series = new Collection.EventSeries()
            {
                Id = id,
                Code = "TestCode",
                Description = "TestDescription",
                StartDateUTC = DateTime.UtcNow
            };
            this.Repository.Setup(x => x.Find(id)).Returns(Task.FromResult(series));
            this.CreateTarget();

            // Act   
            var action = this.Target.Get(id);

            // Assert               
            var result = await action.GetContentAsync<EventSeries>();
            Assert.Equal(series.Id, result.Id);
            Assert.Equal(series.Code, result.Code);
            Assert.Equal(series.Description, result.Description);
            Assert.Equal(series.StartDateUTC, result.StartDateUTC);
        }

        [Fact]
        public async Task Returns_404_For_Invalid_Id()
        {
            // Arrange    
            int invalidId = 12345;
            this.Repository.Setup(x => x.Find(invalidId)).Returns(Task.FromResult<Collection.EventSeries>(null)); // Simulate no match 
            this.CreateTarget();

            // Act    
            var response = await Target.Get(invalidId).GetResponseAsync();

            // Assert      
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public void Returns_All_Series()
        {
            // Arrange 
            var series = new[]
            {
                RandomSeries(),
                RandomSeries(),
                RandomSeries(),
                RandomSeries(),
                RandomSeries(),
            };
            this.Repository.Setup(x => x.All).Returns(series.AsQueryable());
            this.CreateTarget();

            // Act
            var result = Target.Get().ToList(); ;

            // Assert
            Assert.Equal(series.Length, result.Count);
            foreach (var item in series)
            {
                Assert.DoesNotThrow(() => result.Single(x => x.Id == item.Id));
            }
        }

        [Fact]
        public async Task Doesnt_do_double_inserts()
        {
            // Arrange
            var id = TestHelper.RandomInt();
            var transaction = Guid.NewGuid();
            var series = new Collection.EventSeries() { Id = id, RowGUID = transaction };
            var seriesdto = new EventSeriesBindingModel() { Code = "test", Description = "test2", TransactionGuid = transaction };
            var route = TestHelper.RandomRoute();

            Repository
                .Setup(x => x.All)
                .Returns(() => new[] { series }.AsQueryable());
            this.SetupLink(route);


            // Act
            this.CreateTarget();
            var response = await Target.Post(seriesdto).GetResponseAsync();
            int responseid;

            // Assert
            Assert.True(response.TryGetContentValue(out responseid));
            Assert.Equal(HttpStatusCode.SeeOther, response.StatusCode);
            Assert.Equal(id, responseid);          
            this.UOW.Verify(x => x.Save(), Times.Never());
            VerifyLink(r => r["controller"].ToString() == Route.SERIES_CONTROLLER && r["id"].ToString() == id.ToString());
        }

        [Fact]
        public async Task Inserts_A_New_Series()
        {
            // Arrange 
            var series = new EventSeriesBindingModel();
            var id = Rnd.Next();
            var route = TestHelper.RandomRoute();

            Repository.Setup(x => x.Insert(It.IsAny<Collection.EventSeries>()))
                .Callback<Collection.EventSeries>(s => s.Id = id);
            SetupLink(route);

            this.CreateTarget();

            // Act
            var action = await Target.Post(series);
            var result = await action.ExecuteAsync(CancellationToken.None);
            int seriesId;

            // Assert
            Assert.True(result.TryGetContentValue(out seriesId));
            Assert.Equal(id, seriesId);
            this.UOW.Verify(x => x.Save(), Times.Once());   
            VerifyLink(r => r["controller"].ToString() == Route.SERIES_CONTROLLER && r["id"].ToString() == id.ToString());
        }
    }
}
