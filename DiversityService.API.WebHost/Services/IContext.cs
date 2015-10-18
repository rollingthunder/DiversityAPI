namespace DiversityService.API.Services
{
    using System;
    using DiversityService.DB.Collection;

    public interface IFieldDataContext : IDisposable
    {
        IStore<Event, int> Events
        {
            get;
        }

        IStore<IdentificationUnitGeoAnalysis, IdentificationGeoKey> IdentificationGeoAnalyses
        {
            get;
        }

        IStore<Identification, IdentificationKey> Identifications
        {
            get;
        }

        IStore<IdentificationUnit, IdentificationUnitKey> IdentificationUnits
        {
            get;
        }

        int? ProjectId
        {
            get; set;
        }

        IProjectStore Projects
        {
            get;
        }

        IStore<EventSeries, int> Series
        {
            get;
        }

        IStore<Specimen, int> Specimen
        {
            get;
        }

        ITransaction BeginTransaction();
    }

    public interface ITransaction : IDisposable
    {
        void Commit();
    }
}