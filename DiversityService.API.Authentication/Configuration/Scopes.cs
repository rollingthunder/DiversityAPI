﻿using System;
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
                    Enabled = true,
                    Name = "diversityapi",
                    DisplayName = "DiversityAPI",
                    Type = ScopeType.Resource
                }
            });
        }
    }
}