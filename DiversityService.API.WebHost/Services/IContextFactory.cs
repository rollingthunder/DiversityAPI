namespace DiversityService.API.Services
{
    using DiversityService.API.Model.Internal;
    using System.Threading.Tasks;

    public interface IContextFactory
    {
        Task<IFieldDataContext> CreateContextAsync(CollectionServerLogin server);
    }
}