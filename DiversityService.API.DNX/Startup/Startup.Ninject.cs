using System;
using Ninject;

namespace DiversityService.API.DNX
{
    public partial class Startup
    {
        private NinjectControllerActivator ConfigureNinject(IServiceProvider services)
        {
            return new NinjectControllerActivator(new StandardKernel());
        }
    }
}