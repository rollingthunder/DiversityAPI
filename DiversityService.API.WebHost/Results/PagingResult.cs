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
        private readonly Lazy<NegotiatedContentResult<IQueryable<T>>> InnerResult;

        public IQueryable<T> Query { get; private set; }

        public class PageDescriptor
        {
            public uint? skip { get; set; }

            public uint? take { get; set; }
        }

        public const uint DEFAULT_SKIP = 0;
        public const uint DEFAULT_TAKE = 20;
        public const uint MAX_TAKE = 50;

        public PagingResult(HttpStatusCode statusCode, IOrderedQueryable<T> content, ApiController controller)
            : this(statusCode, content,
            controller.Configuration.Services.GetContentNegotiator(),
            controller.Request,
            controller.Configuration.Formatters
            )
        {
        }

        public PagingResult(HttpStatusCode statusCode, IOrderedQueryable<T> content, IContentNegotiator negotiator, HttpRequestMessage request, IEnumerable<MediaTypeFormatter> formatters)
        {
            Query = PageContent(content, request);

            InnerResult = new Lazy<NegotiatedContentResult<IQueryable<T>>>(() =>
            {
                return new NegotiatedContentResult<IQueryable<T>>(statusCode, Query, negotiator, request, formatters);
            });
        }

        private static IQueryable<T> PageContent(IOrderedQueryable<T> content, HttpRequestMessage request)
        {
            PageDescriptor descriptor = new PageDescriptor();
            if (request != null && request.RequestUri != null)
            {
                request.RequestUri.TryReadQueryAs<PageDescriptor>(out descriptor);
            }

            if (descriptor.take.HasValue && descriptor.take > MAX_TAKE)
            {
                descriptor.take = MAX_TAKE;
            }

            return content
                .Skip((int)(descriptor.skip ?? DEFAULT_SKIP))
                .Take((int)(descriptor.take ?? DEFAULT_TAKE));
        }

        public Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken)
        {
            return InnerResult.Value.ExecuteAsync(cancellationToken);
        }
    }
}