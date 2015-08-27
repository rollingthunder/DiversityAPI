namespace DiversityService.API.WebHost
{
    using AutoMapper;
    using Ninject.Modules;

    public class MapperModule : NinjectModule
    {
        public override void Load()
        {
            Mapper.Initialize(MapperConfig.Configure);

            Bind<IMappingEngine>()
                .ToConstant(Mapper.Engine);
        }
    }
}