//namespace DiversityService.API.Test
//{
//    using DiversityService.API.WebHost;
//    using DiversityService.API.Model;
//    using DiversityService.API.Controllers;
//    using DiversityService.API.Services;
//    using Moq;
//    using System;
//    using System.Collections.Generic;
//    using System.Linq;
//    using System.Linq.Expressions;
//    using System.Net;
//    using System.Net.Http;
//    using System.Text;
//    using System.Threading;
//    using System.Threading.Tasks;
//    using System.Web.Http;
//    using System.Web.Http.Routing;
//    using Xunit;

//    public class SeriesFeature
//    {
//        private readonly Mock<IStore<Collection.EventSeries>> MockSeriesStore;
//        private readonly Mock<IMappingService> MockMapping;

//        private readonly HttpRequestMessage Request;
//        private readonly HttpClient Client;

//        private readonly IEnumerable<Collection.EventSeries> FakeSeries;

//        public SeriesFeature()
//        {
//            this.MockSeriesStore = new Mock<IStore<Collection.EventSeries>>();

//            this.MockMapping = new Mock<IMappingService>();

//            Request = new HttpRequestMessage();
//            //Request.Headers.Accept.Add(
//            //    new MediaTypeWithQualityHeaderValue("application/vnd.issue+json"));
//            //IssueLinks = new IssueLinkFactory(Request);
//            //StateFactory = new IssueStateFactory(IssueLinks);
//            FakeSeries = GetFakeSeries();
//            var config = new HttpConfiguration();
//            WebApiConfig.RegisterRoutes(config);
//            var server = new HttpServer(config);
//            Client = new HttpClient(server);
//        }

//        private IEnumerable<Collection.EventSeries> GetFakeSeries()
//        {
//            return new[] {
//                new Collection.EventSeries() {
//                    Id = 1,

//                },
//                new Collection.EventSeries() {
//                    Id = 2
//                }
//            };
//        }

//    }
//}