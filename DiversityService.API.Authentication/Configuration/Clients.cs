using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer3.Core.Models;

namespace DiversityService.API.Authentication
{
    static class Clients
    {
        internal static IEnumerable<Client> Get()
        {
            return new List<Client>
            {
                new Client
                {
                    ClientName = "Diversity API",
                    ClientId = "diversityapi",
                    Enabled = true,
                    AccessTokenType = AccessTokenType.Jwt,

                    Flow = Flows.Implicit,

                    RedirectUris = new List<string>()
                    {
                        "http://localhost:50300/signin-oidc"
                    },

                    AllowedScopes = new List<string>
                    {
                        "openid",
                        "email",
                        "profile",

                        "diversityapi"
                    }
                }
            };
        }
    }
}
