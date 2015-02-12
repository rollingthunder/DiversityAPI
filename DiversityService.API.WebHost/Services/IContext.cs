﻿namespace DiversityService.API.Services
{
    using DiversityService.Collection;
    using System;
    using System.Threading.Tasks;

    public interface ITransaction : IDisposable
    {
        void Commit();
    }

    public interface IContext : IDisposable
    {
        int? ProjectId { get; set; }

        IProjectStore Projects { get; }

        IStore<Event, int> Events { get; }

        IStore<EventSeries, int> Series { get; }

        IStore<Specimen, int> Specimen { get; }

        IStore<IdentificationUnit, IdentificationUnitKey> IdentificationUnits { get; }

        IStore<Identification, IdentificationKey> Identifications { get; }

        ITransaction BeginTransaction();
    }
}