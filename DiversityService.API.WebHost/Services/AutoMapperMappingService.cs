namespace DiversityService.API.Services
{
    using AutoMapper;
    using AutoMapper.QueryableExtensions;
    using System.Linq;

    public class AutoMapperMappingService : IMappingService
    {
        public TDestination Map<TDestination>(object source)
        {
            return Mapper.Map<TDestination>(source);
        }

        public IQueryable<TDestination> Project<TSource, TDestination>(IQueryable<TSource> sourceQuery)
        {
            return sourceQuery.Project<TSource>().To<TDestination>();
        }
    }
}