using DiversityService.API.Model;

namespace DiversityService.API.Services
{
    public interface IContext
    {
        int? ProjectId { get; set; }

        IProjectStore Projects { get; }

        IStore<Collection.Event, int> Events { get; }

        IStore<Collection.EventSeries, int> Series { get; }
    }
}