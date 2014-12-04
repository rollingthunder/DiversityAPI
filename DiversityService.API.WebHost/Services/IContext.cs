using DiversityService.API.Model;

namespace DiversityService.API.Services
{
    public interface IContext
    {
        int? ProjectId { get; set; }

        IProjectStore Projects { get; }

        IEventStore Events { get; }
    }
}