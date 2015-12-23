using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Ninject;

namespace DiversityService.API.DNX
{
    public partial class Startup
    {
        NinjectControllerActivator ConfigureNinject(IServiceProvider services)
        {
            return new NinjectControllerActivator(new StandardKernel());
        }
    }
}
