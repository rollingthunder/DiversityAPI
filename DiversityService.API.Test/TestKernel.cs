namespace DiversityService.API.Test
{
    using DiversityService.API.Controllers;
    using DiversityService.API.WebHost;
    using Ninject;
    using Ninject.Modules;

    public class TestKernel : StandardKernel
    {
        public TestKernel(TestData data = null)
        {
            this.Load(new TestModule(data));
        }
    }

    public class TestModule : NinjectModule
    {
        private readonly TestData Data;

        public TestModule(TestData data = null)
        {
            Data = data ?? TestData.Default();
        }

        public override void Load()
        {
            if (Data.Servers != null)
            {
                SetupMocks.Servers(Kernel, Data.Servers);
            }
            SetupMocks.UserStore(Kernel);

            SetupMocks.Context(Kernel);
            SetupMocks.ContextFactory(Kernel, Data);

            SetupMocks.FieldDataStores(Kernel, Data);
        }
    }

    public class TestControllerModule : NinjectModule
    {
        public override void Load()
        {
            Bind<AccountController>()
                .ToSelf()
                .WithPropertyValue("UserManager", (ctx) => ctx.Kernel.Get<ApplicationUserManager>());
        }
    }
}