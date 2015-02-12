namespace DiversityService.API.WebHost
{
    using AutoMapper;
    using DiversityService.API.Model;
    using System;
    using System.Data.Entity.Spatial;

    public static class MapperConfig
    {
        public static void Configure(IConfiguration MappingConfiguration)
        {
            MappingConfiguration.CreateMap<CollectionServerElement, InternalCollectionServer>();

            MappingConfiguration.CreateMap<Collection.Project, Project>()
                .ForMember(mem => mem.Id, map => map.MapFrom(x => x.ProjectID))
                .ForMember(mem => mem.Name, map => map.MapFrom(x => x.DisplayText));

            // Localizations

            // EventSeries
            MappingConfiguration.CreateMap<Collection.EventSeries, EventSeries>();

            MappingConfiguration.CreateMap<EventSeriesBindingModel, Collection.EventSeries>()
                .ForMember(x => x.DateCache, map => map.Ignore())
                // Ignore Navigation Properties
                .ForMember(x => x.Children, map => map.Ignore())
                .ForMember(x => x.Parent, map => map.Ignore())
                .ForMember(x => x.Events, map => map.Ignore())
                .ForMember(x => x.Images, map => map.Ignore());

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

            // Identification <-> IU
            MappingConfiguration.CreateMap<Collection.IdentificationUnit, Identification>()
                .ForMember(x => x.RelatedId, map => map.MapFrom(y => y.RelatedUnitId));

            MappingConfiguration.CreateMap<Identification, Collection.IdentificationUnit>()
                .Include<IdentificationBindingModel, Collection.IdentificationUnit>();

            MappingConfiguration.CreateMap<IdentificationBindingModel, Collection.IdentificationUnit>();

            // Identification <-> ID
            MappingConfiguration.CreateMap<Collection.Identification, Identification>()
                .ForMember(x => x.Date, map => map.MapFrom(y => y.GetCollectionDate()))
                .ForMember(x => x.Id, map => map.MapFrom(y => y.IdentificationUnitID))
                .ForMember(x => x.Uri, map => map.MapFrom(y => y.NameURI))
                .ForSourceMember(x => x.Id, map => map.Ignore());

            MappingConfiguration.CreateMap<Identification, Collection.Identification>()
                .ForMember(x => x.IdentificationUnitID, map => map.MapFrom(y => y.Id))
                .ForMember(x => x.Id, map => map.Ignore());
        }
    }
}