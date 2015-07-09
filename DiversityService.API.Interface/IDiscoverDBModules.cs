﻿namespace DiversityService.API
{
    using DiversityService.API.Model;
    using DiversityService.API.Model.Internal;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IDiscoverDBModules
    {
        Task<IEnumerable<DBModule>> DiscoverModules(CollectionServerLogin Server);
    }
}