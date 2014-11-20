namespace DiversityService.API.Test
{
    using System.Net.Http;
    using System.Threading.Tasks;

    public class TestHandler : HttpMessageHandler
    {
        private HttpResponseMessage Response;

        public TestHandler(HttpResponseMessage response)
        {
            Response = response;
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, System.Threading.CancellationToken cancellationToken)
        {
            return Task.FromResult(Response);
        }
    }
}