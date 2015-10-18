namespace DiversityService.API.Handler
{
    using System;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;

    public class RequireHttpsMessageHandler : DelegatingHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            if (request.RequestUri.Scheme != Uri.UriSchemeHttps)
            {
                HttpResponseMessage response = request.CreateResponse(HttpStatusCode.Redirect);

                var uri = request.RequestUri.AbsoluteUri;
                var uriNoScheme = uri.Split(new[] { ':' }, 2).Last();
                var httpsUri = string.Format("https:{0}", uriNoScheme);

                response.Headers.Location = new Uri(httpsUri);
                response.ReasonPhrase = "SSL Required";
                return Task.FromResult<HttpResponseMessage>(response);
            }
            return base.SendAsync(request, cancellationToken);
        }
    }
}