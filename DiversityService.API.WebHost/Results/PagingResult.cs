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
    using System.Web;
    using System.Web.Http;
    using System.Web.Http.Results;

    public class PagingResult<T> : NegotiatedContentResult<IQueryable<T>>
    {
        public class PageDescriptor
        {
            public uint? skip { get; set; }

            public uint? take { get; set; }
        }

        public const uint DEFAULT_SKIP = 0;
        public const uint DEFAULT_TAKE = 20;
        public const uint MAX_TAKE = 50;

        public PagingResult(HttpStatusCode statusCode, IQueryable<T> content, ApiController controller)
            : base(statusCode, PageContent(content, controller.Request), controller)
        {
        }

        public PagingResult(HttpStatusCode statusCode, IQueryable<T> content, IContentNegotiator contentNegotiator, HttpRequestMessage request, IEnumerable<MediaTypeFormatter> formatters)
            : base(statusCode, PageContent(content, request), contentNegotiator, request, formatters)
        {
        }

        private static IQueryable<T> PageContent(IQueryable<T> content, HttpRequestMessage request)
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
    }
}