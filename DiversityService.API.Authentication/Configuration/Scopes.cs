using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer3.Core.Models;

namespace DiversityService.API.Authentication
{
    static class Scopes
    {
        internal static IEnumerable<Scope> Get()
        {
            return StandardScopes.All.Concat(new[] {
                new Scope()
                {
                    Name = "diversityapi",
                    DisplayName = "DiversityAPI",
                    Claims = new List<ScopeClaim>
                    {
                        new ScopeClaim("role")
                    }
                }
            });
        }
    }
}
