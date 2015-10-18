namespace DiversityService.API.Client.Test
{
    using DiversityService.API.Test;
    using DiversityService.API.WebHost;
    using Microsoft.Owin.Testing;
    using Ninject;
    using System;
    using System.Net.Http;

    internal class TestAPI : IDisposable
    {
        public readonly TestServer Server;
        public readonly IKernel Kernel;
        public readonly APIBrowser Browser;

        public HttpClient HttpClient { get { return Server.HttpClient; } }

        public TestAPI(string relativeBase = "", TestData data = null)
        {
            var Kernel = new TestKernel(data);

            Kernel.Load<TestControllerModule>();

            var startup = new TestStartup(Kernel);

            this.Kernel = Kernel;

            Server = TestServer.Create(startup.Configuration);

            Server.BaseAddress = new Uri("https://diversityapi.de/api" + relativeBase);

            Browser = new APIBrowser(HttpClient);
        }

        public void Dispose()
        {
            Server.Dispose();
            Kernel.Dispose();
        }
    }
}