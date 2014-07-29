using DiversityService.API.Model;
using DiversityService.API.WebHost.Controllers;
using Ninject.Modules;
using Ninject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DiversityService.API.WebHost
{
    public class ControllerModule : NinjectModule
    {
        public override void Load()
        {
            Bind<CollectionController>()
                .ToSelf()
                .WithConstructorArgument(typeof(IEnumerable<InternalCollectionServer>), ctx => ctx.Kernel.Get<IEnumerable<InternalCollectionServer>>());

            Bind<AccountController>()
                .ToSelf()
                .WithConstructorArgument(typeof(IEnumerable<InternalCollectionServer>), ctx => ctx.Kernel.Get<IEnumerable<InternalCollectionServer>>());
        }
    }
}