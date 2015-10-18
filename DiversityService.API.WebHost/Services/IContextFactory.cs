namespace DiversityService.API.Services
{
    using System.Threading.Tasks;
    using DiversityService.API.Model.Internal;

    public interface IContextFactory
    {
        Task<IFieldDataContext> CreateContextAsync(CollectionServerLogin server);
    }
}