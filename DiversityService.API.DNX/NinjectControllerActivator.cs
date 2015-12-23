using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
using Microsoft.AspNet.Mvc.Controllers;
using Ninject;

namespace DiversityService.API.DNX
{
    public class NinjectControllerActivator : IControllerActivator
    {
        IKernel Kernel;

        public NinjectControllerActivator(IKernel kernel)
        {
            this.Kernel = kernel;
        }
        public object Create(ActionContext context, Type controllerType)
        {
            return Kernel.Get(controllerType);
        }
    }
}
