namespace DiversityService.API.Test
{
    using DiversityService.API.WebHost.Models;
    using Microsoft.AspNet.Identity;
    using Ninject;
    using Ninject.Extensions.Factory;
    using Ninject.MockingKernel.Moq;
    using Ninject.Modules;

    public class TestKernel : MoqMockingKernel
    {
        public TestKernel()
        {
            //this.Load<FuncModule>();

            this.Bind<IUserStore<ApplicationUser>>().To<TestUserStore>();
        }
    }
}