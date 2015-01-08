using DiversityService.API.Model;
using System;
using System.Threading.Tasks;

namespace DiversityService.API.Services
{
    public interface IContext : IDisposable
    {
        Task SubmitAsync();

        int? ProjectId { get; set; }

        IProjectStore Projects { get; }

        IStore<Collection.Event, int> Events { get; }

        IStore<Collection.EventSeries, int> Series { get; }

        IStore<Collection.Specimen, int> Specimen { get; }
    }
}