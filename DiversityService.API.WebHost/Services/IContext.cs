using DiversityService.API.Model;

namespace DiversityService.API.Services
{
    public interface IContext
    {
        IProjectStore Projects { get; }
    }
}