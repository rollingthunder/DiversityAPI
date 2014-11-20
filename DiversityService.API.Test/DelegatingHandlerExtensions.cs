namespace DiversityService.API.Test
{
    using System.Net;
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;

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