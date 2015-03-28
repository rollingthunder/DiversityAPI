namespace DiversityService.API.Client.Test
{
    using DiversityService.API.WebHost;
    using Microsoft.Owin.Testing;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    internal static class TestAPI
    {
        public static TestServer Create()
        {
            var api = TestServer.Create<TestStartup>();

            api.BaseAddress = new Uri("https://diversityapi.de/");

            return api;
        }
    }
}