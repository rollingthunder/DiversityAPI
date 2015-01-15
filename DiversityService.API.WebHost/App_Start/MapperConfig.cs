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

            MappingConfiguration.CreateMap<EventSeriesBindingModel, Collection.EventSeries>();

            // Event
            MappingConfiguration.CreateMap<Collection.Event, Event>();

            MappingConfiguration.CreateMap<Event, Collection.Event>()
                .Include<EventBindingModel, Collection.Event>();

            MappingConfiguration.CreateMap<EventBindingModel, Collection.Event>();

            // Specimen
            MappingConfiguration.CreateMap<Collection.Specimen, Specimen>()
                .ForMember(x => x.CollectionDate, map => map.Ignore())
                .AfterMap((entity, dto) => dto.CollectionDate = entity.GetCollectionDate());

            MappingConfiguration.CreateMap<Specimen, Collection.Specimen>()
                .ForSourceMember(x => x.CollectionDate, map => map.Ignore())
                .AfterMap((dto, entity) => entity.SetCollectionDate(dto.CollectionDate))
                .Include<SpecimenBindingModel, Collection.Specimen>();

            MappingConfiguration.CreateMap<SpecimenBindingModel, Collection.Specimen>();

            // Identification
            MappingConfiguration.CreateMap<Collection.IdentificationUnit, Identification>();

            MappingConfiguration.CreateMap<Identification, Collection.IdentificationUnit>()
                .Include<IdentificationBindingModel, Collection.IdentificationUnit>();

            MappingConfiguration.CreateMap<IdentificationBindingModel, Collection.IdentificationUnit>();
        }
    }
}