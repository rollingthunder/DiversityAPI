namespace DiversityService.API.Test
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Web.Http;
    using System.Web.Http.Hosting;

    internal static class DelegatingHandlerExtensions
    {
        internal static Task<HttpResponseMessage> InvokeAsync(this DelegatingHandler handler, HttpRequestMessage request, CancellationToken cancellationToken = default(CancellationToken))
        {
            handler.InnerHandler = new TestHandler(new HttpResponseMessage(HttpStatusCode.OK));
            var invoker = new HttpMessageInvoker(handler);
            return invoker.SendAsync(request, cancellationToken);
        }
    }
}