using AutoMapper;
using DiversityService.API.Model;
using DiversityService.API.WebHost;
using DiversityService.API.WebHost.Controllers;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
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
        private readonly IMappingEngine Mapping;
        private SeriesController Target;

        public SeriesControllerTest()
        {
            this.Repository = new Mock<IRepository<Collection.EventSeries>>();
            this.UOW = new Mock<IUnitOfWork>();
            Mapper.CreateMap<Collection.EventSeries, EventSeries>();
            this.Mapping = Mapper.Engine;
        }

        private void CreateTarget()
        {
            var repoFactory = new Mock<IRepositoryFactory>();
            repoFactory.Setup(x => x.Get<Collection.EventSeries>())
                .Returns(Repository.Object);
                
            this.Target = new SeriesController(repoFactory.Object, this.Mapping);
            this.Target.EnsureNotNull();
        }

        [Fact]
        public void Returns_A_Series_By_Id()
        {
            // Arrange        
            int id = 12345;
            var employee = new Collection.EventSeries()
            {
                Id = id,
                SeriesCode = "TestCode",
                Description = "TestDescription"
            };
            this.Repository.Setup(x => x.Find(id)).Returns(employee);
            this.CreateTarget();

            // Act   
            HttpResponseMessage response = this.Target.Get(id);

            // Assert   
            Assert.NotNull(response);
            Assert.NotNull(response.Content);
            Assert.IsType(typeof(ObjectContent<EventSeries>), response.Content);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var content = (response.Content as ObjectContent<EventSeries>);
            var result = content.Value as EventSeries;
            Assert.Equal(employee.Id, result.Id);
            Assert.Equal(employee.SeriesCode, result.SeriesCode);
            Assert.Equal(employee.Description, result.Description);

        }

        [Fact]
        public void Returns_404_For_Invalid_Id()
        {
            // Arrange    
            int invalidId = 12345;
            this.Repository.Setup(x => x.Find(invalidId)).Returns<Collection.EventSeries>(null); // Simulate no match 
            this.CreateTarget();

            // Act    
            HttpResponseMessage response = null;
            try
            {
                response = this.Target.Get(invalidId);
                Assert.True(false, "Should Have Thrown");
            }
            catch (HttpResponseException ex)
            {
                // Assert      
                Assert.Equal(HttpStatusCode.NotFound, ex.Response.StatusCode);
            }
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
        public async void Inserts_A_New_Series()
        {
            // Arrange 
            var series = new EventSeries();
            var id = Rnd.Next();
            Repository.Setup(x => x.Insert(It.IsAny<Collection.EventSeries>()))
                .Callback<Collection.EventSeries>(s => s.Id = id);
            this.CreateTarget();

            // Act
            var action = await Target.Post(series);
            var result = await action.ExecuteAsync(CancellationToken.None);
            int seriesId;

            // Assert
            Assert.True(result.TryGetContentValue(out seriesId));
            Assert.Equal(id, seriesId);
        }
    }
}
