namespace DiversityService.API.WebHost
{
    using AutoMapper;
    using DiversityService.API.Model;
    using DiversityService.API.Model.Internal;
    using DiversityService.API.Services;
    using NodaTime;
    using System;
    using System.Collections.Generic;
    using System.Data.Entity.Spatial;
    using Collection = DiversityService.DB.Collection;

    public static class MapperConfig
    {
        public static void Configure(IConfiguration mappingConfiguration)
        {
            mappingConfiguration.CreateMap<CollectionServerElement, InternalCollectionServer>();
            mappingConfiguration.CreateMap<ServerLoginElement, CollectionServerLogin>();
            mappingConfiguration.CreateMap<ServerLoginCatalogElement, CollectionServerLogin>()
                .IncludeBase<ServerLoginElement, CollectionServerLogin>();

            mappingConfiguration.CreateMap<Collection.Project, Project>()
                .ForMember(mem => mem.Id, map => map.MapFrom(x => x.ProjectID))
                .ForMember(mem => mem.Name, map => map.MapFrom(x => x.DisplayText));

            // Times
            mappingConfiguration.CreateMap<LocalDate, DateTime>()
                .ConvertUsing(x => x.AtMidnight().ToDateTimeUnspecified());

            // Localizations
            mappingConfiguration.CreateMap<DbGeography, Localization>()
                .ConvertUsing(x => x.ToLocalization());

            mappingConfiguration.CreateMap<Localization, DbGeography>()
                .ConvertUsing(x => x.ToGeography());

            mappingConfiguration.CreateMap<DbGeography, IEnumerable<Localization>>()
                .ConvertUsing(x => x.ToTour());

            mappingConfiguration.CreateMap<IEnumerable<Localization>, DbGeography>()
                .ConvertUsing(x => x.ToGeography());

            // EventSeries
            mappingConfiguration.CreateMap<Collection.EventSeries, EventSeries>();

            mappingConfiguration.CreateMap<EventSeriesBindingModel, Collection.EventSeries>()
                .ForMember(x => x.DateCache, map => map.Ignore())
                // Ignore Navigation Properties
                .ForMember(x => x.Children, map => map.Ignore())
                .ForMember(x => x.Parent, map => map.Ignore())
                .ForMember(x => x.Events, map => map.Ignore())
                .ForMember(x => x.Images, map => map.Ignore());

            // Event
            mappingConfiguration.CreateMap<Collection.Event, Event>();

            mappingConfiguration.CreateMap<Event, Collection.Event>()
                .Include<EventBindingModel, Collection.Event>();

            mappingConfiguration.CreateMap<EventBindingModel, Collection.Event>();

            // Specimen
            mappingConfiguration.CreateMap<Collection.Specimen, Specimen>()
                .ForMember(x => x.CollectionDate, map => map.Ignore())
                .AfterMap((entity, dto) => dto.CollectionDate = entity.GetCollectionDate());

            mappingConfiguration.CreateMap<Specimen, Collection.Specimen>()
                .ForSourceMember(x => x.CollectionDate, map => map.Ignore())
                .AfterMap((dto, entity) => entity.SetCollectionDate(dto.CollectionDate))
                .Include<SpecimenBindingModel, Collection.Specimen>();

            mappingConfiguration.CreateMap<SpecimenBindingModel, Collection.Specimen>();

            // Identification <-> IU
            mappingConfiguration.CreateMap<Collection.IdentificationUnit, Identification>();

            mappingConfiguration.CreateMap<Identification, Collection.IdentificationUnit>()
                .Include<IdentificationBindingModel, Collection.IdentificationUnit>();

            mappingConfiguration.CreateMap<IdentificationBindingModel, Collection.IdentificationUnit>();

            // Identification <-> ID
            mappingConfiguration.CreateMap<Collection.Identification, Identification>()
                .ForMember(x => x.Date, map => map.MapFrom(y => y.GetCollectionDate()))
                .ForMember(x => x.Id, map => map.MapFrom(y => y.IdentificationUnitID))
                .ForMember(x => x.Uri, map => map.MapFrom(y => y.NameURI))
                .ForSourceMember(x => x.Id, map => map.Ignore());

            mappingConfiguration.CreateMap<Identification, Collection.Identification>()
                .ForMember(x => x.IdentificationUnitID, map => map.MapFrom(y => y.Id))
                .ForMember(x => x.NameURI, map => map.MapFrom(y => y.Uri))
                .ForMember(x => x.TaxonomicName, map => map.MapFrom(y => y.Name))
                // Map Date to components
                .AfterMap((id, entity) => entity.SetCollectionDate(id.Date))
                .ForMember(x => x.IdentificationDate, map => map.Ignore())
                .ForMember(x => x.IdentificationDateCategory, map => map.Ignore())
                .ForMember(x => x.IdentificationDay, map => map.Ignore())
                .ForMember(x => x.IdentificationMonth, map => map.Ignore())
                .ForMember(x => x.IdentificationYear, map => map.Ignore())
                // Ignore Unmapped Properties
                .ForMember(x => x.Id, map => map.Ignore())
                .ForMember(x => x.IdentificationCategory, map => map.Ignore())
                .ForMember(x => x.IdentificationDateSupplement, map => map.Ignore())
                .ForMember(x => x.Notes, map => map.Ignore())
                .ForMember(x => x.ReferenceDetails, map => map.Ignore())
                .ForMember(x => x.ReferenceTitle, map => map.Ignore())
                .ForMember(x => x.ReferenceURI, map => map.Ignore())
                .ForMember(x => x.ResponsibleAgentURI, map => map.Ignore())
                .ForMember(x => x.ResponsibleName, map => map.Ignore())
                .ForMember(x => x.RowGUID, map => map.Ignore())
                .ForMember(x => x.TypeNotes, map => map.Ignore())
                .ForMember(x => x.TypeStatus, map => map.Ignore())
                .ForMember(x => x.VernacularTerm, map => map.Ignore())
                // TODO Add to DTO
                .ForMember(x => x.IdentificationQualifier, map => map.Ignore());

            // Identification <-> IUGAN
            mappingConfiguration.CreateMap<Collection.IdentificationUnitGeoAnalysis, Identification>()
                .ForMember(x => x.Localization, map => map.MapFrom(y => y.Geography))
                .ForMember(x => x.Id, map => map.MapFrom(y => y.IdentificationUnitId))
                // Ignore Properties Not mapped from IUGAN
                .ForMember(x => x.Date, map => map.Ignore())
                .ForMember(x => x.Name, map => map.Ignore())
                .ForMember(x => x.RelatedId, map => map.Ignore())
                .ForMember(x => x.RelationType, map => map.Ignore())
                .ForMember(x => x.TaxonomicGroup, map => map.Ignore())
                .ForMember(x => x.Uri, map => map.Ignore());

            mappingConfiguration.CreateMap<Identification, Collection.IdentificationUnitGeoAnalysis>()
                .ForMember(x => x.Geography, map => map.MapFrom(y => y.Localization))
                .ForMember(x => x.IdentificationUnitId, map => map.MapFrom(y => y.Id))
                .ForMember(x => x.AnalysisDate, map => map.MapFrom(y => y.Date))
                // Ignore Properties Not mapped to IUGAN
                .ForMember(x => x.Geometry, map => map.Ignore())
                .ForMember(x => x.Notes, map => map.Ignore())
                .ForMember(x => x.ResponsibleAgentURI, map => map.Ignore())
                .ForMember(x => x.ResponsibleName, map => map.Ignore())
                .ForMember(x => x.RowGUID, map => map.Ignore())
                // Ignore Navigation Properties
                .ForMember(x => x.IdentificationUnit, map => map.Ignore());

            // TaxonList -> Model.TaxonList
            mappingConfiguration.CreateMap<DB.TaxonNames.TaxonList, TaxonList>()
                .ForMember(x => x.Name, map => map.MapFrom(y => y.DisplayText))
                .ForMember(x => x.TaxonGroup, map => map.MapFrom(y => y.TaxonomicGroup))
                .ForMember(x => x.Id, map => map.MapFrom(y => y.GetSHA1Hash()));
        }
    }
}