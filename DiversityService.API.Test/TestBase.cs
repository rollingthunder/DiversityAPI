namespace DiversityService.API.Test
{
    using AutoMapper;
    using AutoMapper.Internal;
    using AutoMapper.Mappers;
    using DiversityService.API.Services;
    using DiversityService.API.WebHost;

    public class TestBase
    {
        private readonly static IMappingEngine Engine;

        static TestBase()
        {
            // MappingService is not really mockable
            // So we will use the real one for testing
            var mapperConfig = new ConfigurationStore(new TypeMapFactory(), MapperRegistry.Mappers);
            MapperConfig.Configure(mapperConfig);
            Engine = new AutoMapper.MappingEngine(mapperConfig);
        }

        protected TestKernel Kernel = new TestKernel();

        public TestBase()
        {
            Kernel
                .Bind<IMappingEngine>()
                .ToConstant<IMappingEngine>(Engine);
        }
    }
}