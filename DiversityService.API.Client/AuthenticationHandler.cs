using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DiversityService.API.Client
{
    public class AuthenticationHandler : DelegatingHandler
    {
        private readonly Func<Task<string>> getToken;

        public AuthenticationHandler(HttpMessageHandler innerHandler, Func<Task<string>> getToken)
            : base(innerHandler)
        {
            if (getToken == null) throw new ArgumentNullException("getToken");
            this.getToken = getToken;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            // See if the request has an authorize header
            var auth = request.Headers.Authorization;
            if (auth != null)
            {
                var token = await getToken().ConfigureAwait(false);
                request.Headers.Authorization = new AuthenticationHeaderValue(auth.Scheme, token);
            }

            return await base.SendAsync(request, cancellationToken).ConfigureAwait(false);
        }
    }
}