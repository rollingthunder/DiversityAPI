namespace DiversityService.API.WebHost
{
    using AutoMapper;
    using DiversityService.API.Model;
    using System;

    public static class MapperConfig
    {
        public static void Configure(IConfiguration MappingConfiguration)
        {
            MappingConfiguration.CreateMap<CollectionServerElement, InternalCollectionServer>();

            MappingConfiguration.CreateMap<Collection.EventSeries, EventSeries>();

            MappingConfiguration.CreateMap<EventSeriesCommon, Collection.EventSeries>()
                .Include<EventSeriesBindingModel, Collection.EventSeries>()
                .Include<EventSeries, Collection.EventSeries>();
            MappingConfiguration.CreateMap<EventSeriesBindingModel, Collection.EventSeries>()
                .ForMember(es => es.RowGUID, map => map.MapFrom(es => es.TransactionGuid));
            MappingConfiguration.CreateMap<EventSeries, Collection.EventSeries>();
        }
    }
}