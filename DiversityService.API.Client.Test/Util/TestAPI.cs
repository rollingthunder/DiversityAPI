namespace DiversityService.API.Client.Test
{
    using DiversityService.API.Model;
    using DiversityService.API.Services;
    using DiversityService.API.Test;
    using DiversityService.API.WebHost;
    using Microsoft.Owin.Testing;
    using Ninject;
    using Ninject.MockingKernel;
    using Ninject.MockingKernel.Moq;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using System.Text;
    using System.Threading.Tasks;

    internal class TestAPI : IDisposable
    {
        public readonly TestServer Server;
        public readonly MoqMockingKernel Kernel;
        public readonly APIBrowser Browser;

        public HttpClient HttpClient { get { return Server.HttpClient; } }

        public TestAPI(string relativeBase = "")
        {
            Kernel = new MoqMockingKernel();

            var startup = new TestStartup(Kernel);

            //Mocks.SetupMocks(Kernel, TestData.Default());

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