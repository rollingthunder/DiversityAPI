namespace DiversityService.API.Test
{
    using DiversityService.API.WebHost.Models;
    using Microsoft.AspNet.Identity;
    using Ninject.MockingKernel.Moq;

    public class TestKernel : MoqMockingKernel
    {
        public TestKernel()
        {
            //this.Load<FuncModule>();

            this.Bind<IUserStore<ApplicationUser>>().To<TestUserStore>();
        }
    }
}