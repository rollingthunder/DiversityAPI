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

            MappingConfiguration.CreateMap<Collection.Project, Project>()
                .ForMember(mem => mem.Id, map => map.MapFrom(x => x.ProjectID))
                .ForMember(mem => mem.Name, map => map.MapFrom(x => x.DisplayText));

            // EventSeries
            MappingConfiguration.CreateMap<Collection.EventSeries, EventSeries>();

            MappingConfiguration.CreateMap<EventSeries, Collection.EventSeries>()
                .Include<EventSeriesBindingModel, Collection.EventSeries>();

            // Event
            MappingConfiguration.CreateMap<Collection.Event, Event>();

            MappingConfiguration.CreateMap<Event, Collection.Event>()
                .Include<EventBindingModel, Collection.Event>();
        }
    }
}