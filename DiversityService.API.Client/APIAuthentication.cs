using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiversityService.API.Client
{
    internal class APIAuthentication
    {
        private HttpClient Client;

        public APIAuthentication(HttpClient client)
        {
            Contract.Requires<ArgumentNullException>(client != null);

            Client = client;
        }


    }
}