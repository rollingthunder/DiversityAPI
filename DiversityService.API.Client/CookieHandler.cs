namespace DiversityService.API.Client
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Net;
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;

    internal static class CookieContainerExtensions
    {
        internal static void SetCookies(this CookieContainer container, HttpResponseMessage response, Uri requestUri = null)
        {
            Contract.Requires(container != null);
            Contract.Requires(response != null);

            IEnumerable<string> cookieHeaders;
            if (response.Headers.TryGetValues("Set-Cookie", out cookieHeaders))
            {
                foreach (string cookie in cookieHeaders)
                {
                    container.SetCookies(requestUri ?? response.RequestMessage.RequestUri, cookie);
                }
            }
        }

        internal static void ApplyCookies(this CookieContainer container, HttpRequestMessage request)
        {
            Contract.Requires(container != null);
            Contract.Requires(request != null);

            string cookieHeader = container.GetCookieHeader(request.RequestUri);
            if (!string.IsNullOrEmpty(cookieHeader))
            {
                request.Headers.TryAddWithoutValidation("Cookie", cookieHeader);
            }
        }
    }

    /// <summary>
    /// Simple Handler that implements Cookie functionality
    /// for the client.
    /// Sets new cookies from incoming replies and adds applicable cookies to
    /// outgoing requests.
    /// </summary>
    public class CookieHandler : DelegatingHandler
    {
        public CookieContainer Container { get; private set; }

        public CookieHandler(CookieContainer container = null)
        {
            Container = container ?? new CookieContainer();
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            this.Container.ApplyCookies(request);
            var response = await base.SendAsync(request, cancellationToken);
            this.Container.SetCookies(response);
            return response;
        }
    }
}