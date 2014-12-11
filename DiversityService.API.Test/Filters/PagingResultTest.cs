namespace DiversityService.API.Test
{
    using DiversityService.API.Filters;
    using DiversityService.API.Results;
    using DiversityService.API.Services;
    using Ninject;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Net.Http.Formatting;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Web.Http.Filters;
    using Xunit;

    public class PagingResultTest : TestBase
    {
        private HttpRequestMessage Request;

        private PagingResult<int> Result;

        public PagingResultTest()
        {
            Request = new HttpRequestMessage();
        }

        private PagingResult<T> CreateResult<T>(IQueryable<T> content)
        {
            return new PagingResult<T>(
                HttpStatusCode.OK,
                content,
                new DefaultContentNegotiator(),
                Request,
                Enumerable.Empty<MediaTypeFormatter>()
                );
        }

        [Fact]
        public void Applies_Defaults_Without_Uri()
        {
            // Arrange
            var data = Enumerable.Range(1, 100);

            Request.RequestUri = null;

            // Act
            Result = CreateResult(data.AsQueryable());

            // Assert
            Assert.Equal(PagingResult<int>.DEFAULT_TAKE, (uint)Result.Content.Count());
            Assert.Equal(Enumerable.Range(1, (int)PagingResult<int>.DEFAULT_TAKE), Result.Content);
        }

        [Fact]
        public void Applies_Defaults_Without_Explicit_Query_Parameter()
        {
            // Arrange
            var data = Enumerable.Range(1, 100);

            SetPageQuery(null, null);

            // Act
            Result = CreateResult(data.AsQueryable());

            // Assert
            Assert.Equal(PagingResult<int>.DEFAULT_TAKE, (uint)Result.Content.Count());
            Assert.Equal(Enumerable.Range(1, (int)PagingResult<int>.DEFAULT_TAKE), Result.Content);
        }

        [Fact]
        public void Applies_Skip_With_Explicit_Query_Parameter()
        {
            // Arrange
            var data = Enumerable.Range(0, 100);

            SetPageQuery(null, 10);

            // Act
            Result = CreateResult(data.AsQueryable());

            // Assert
            Assert.Equal(PagingResult<int>.DEFAULT_TAKE, (uint)Result.Content.Count());
            Assert.Equal(Enumerable.Range(10, (int)PagingResult<int>.DEFAULT_TAKE), Result.Content);
        }

        [Fact]
        public void Applies_Max_Page_With_Parameter_OOR()
        {
            // Arrange
            var data = Enumerable.Range(1, 100);

            SetPageQuery(1000, null);

            // Act
            Result = CreateResult(data.AsQueryable());

            // Assert
            Assert.Equal(PagingResult<int>.MAX_TAKE, (uint)Result.Content.Count());
            Assert.Equal(Enumerable.Range(1, (int)PagingResult<int>.MAX_TAKE), Result.Content);
        }

        private void SetPageQuery(uint? take, uint? skip)
        {
            string takeQ = string.Empty;
            string skipQ = string.Empty;

            if (take.HasValue)
            {
                takeQ = string.Format("&take={0}", take.Value);
            }

            if (skip.HasValue)
            {
                skipQ = string.Format("&skip={0}", skip.Value);
            }

            Request.RequestUri = new Uri(string.Format("https://localhost/?q=tsrt{0}{1}", takeQ, skipQ), UriKind.Absolute);
        }
    }
}