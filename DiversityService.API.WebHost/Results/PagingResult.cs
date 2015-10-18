namespace DiversityService.API.Results
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Net.Http.Formatting;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Web.Http;
    using System.Web.Http.Results;

    public class PagingResult<T> : IQueryResult<T>
    {
        public const uint DefaultSkip = 0;
        public const uint DefaultTake = 20;
        public const uint MaxTake = 50;

        private readonly Lazy<NegotiatedContentResult<IQueryable<T>>> innerResult;

        public PagingResult(HttpStatusCode statusCode, IOrderedQueryable<T> content, ApiController controller)
            : this(
                statusCode,
                content,
                controller.Configuration.Services.GetContentNegotiator(),
                controller.Request,
                controller.Configuration.Formatters)
        {
        }

        public PagingResult(HttpStatusCode statusCode, IOrderedQueryable<T> content, IContentNegotiator negotiator, HttpRequestMessage request, IEnumerable<MediaTypeFormatter> formatters)
        {
            Query = PageContent(content, request);

            innerResult = new Lazy<NegotiatedContentResult<IQueryable<T>>>(() =>
            {
                return new NegotiatedContentResult<IQueryable<T>>(statusCode, Query, negotiator, request, formatters);
            });
        }

        public IQueryable<T> Query { get; private set; }

        public Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken)
        {
            return innerResult.Value.ExecuteAsync(cancellationToken);
        }

        private static IQueryable<T> PageContent(IOrderedQueryable<T> content, HttpRequestMessage request)
        {
            PageDescriptor descriptor = new PageDescriptor();
            if (request != null && request.RequestUri != null)
            {
                request.RequestUri.TryReadQueryAs<PageDescriptor>(out descriptor);
            }

            if (descriptor.Take.HasValue && descriptor.Take > MaxTake)
            {
                descriptor.Take = MaxTake;
            }

            return content
                .Skip((int)(descriptor.Skip ?? DefaultSkip))
                .Take((int)(descriptor.Take ?? DefaultTake));
        }

        public class PageDescriptor
        {
            public uint? Skip { get; set; }

            public uint? Take { get; set; }
        }
    }
}