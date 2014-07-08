namespace DiversityService.API.WebHost
{
    using AutoMapper;
    using DiversityService.API.Model;

    public static class MapperConfig
    {
        public static void Configure()
        {
            //Mapper.Initialize(cfg =>
            //{
            //    cfg.SourceMemberNamingConvention = new LowerUnderscoreNamingConvention();
            //    cfg.DestinationMemberNamingConvention = new PascalCaseNamingConvention();
            //});

            Mapper.CreateMap<CollectionServerElement, CollectionServer>().AfterMap((a,b) => 
            {
            });

            Mapper.CreateMap<Collection.EventSeries, EventSeries>();
        }
    }
}