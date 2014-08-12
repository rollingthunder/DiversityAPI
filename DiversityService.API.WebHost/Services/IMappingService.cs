namespace DiversityService.API.Services
{
    using System.Linq;
    
    public interface IMappingService
    {
        TDestination Map<TDestination>(object source);

        IQueryable<TDestination> Project<TSource, TDestination>(IQueryable<TSource> sourceQuery);
    }
}
