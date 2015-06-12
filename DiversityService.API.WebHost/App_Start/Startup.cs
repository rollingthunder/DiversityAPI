﻿namespace DiversityService.API.WebHost
{
    using Microsoft.Owin;
    using Ninject;
    using Owin;

    public partial class Startup
    {
        public virtual void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
            ConfigureNinject(app);
            ConfigureWebApi(app);
        }
    }

    public partial class TestStartup : Startup
    {
        public IKernel Kernel
        {
            get { return base.Kernel; }
        }

        public TestStartup()
        {
        }

        public TestStartup(IKernel kernel)
        {
            InitializeKernelIfNecessary(kernel, testing: true);
        }

        public override void Configuration(IAppBuilder app)
        {
            ConfigureTestAuth(app);
            ConfigureNinject(app);
            ConfigureWebApi(app, testing: true);
        }
    }
}