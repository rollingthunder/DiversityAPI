namespace DiversityService.API.Client.Test
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;

    internal class CookieHandler : DelegatingHandler
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