namespace DiversityService.API.Client.Test
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Text;
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
}