namespace DiversityService.API.Controllers
{
    using System;
    using System.Linq;
    using System.Net;
    using System.Threading.Tasks;
    using System.Web.Http;
    using System.Web.Http.Routing;
    using AutoMapper;
    using DiversityService.API.Model;
    using DiversityService.API.Results;

    public abstract class DiversityController : ApiController
    {
        public DiversityController(IMappingEngine mapper)
        {
            Mapper = mapper;
        }

        protected IMappingEngine Mapper
        {
            get; private set;
        }

        protected MappedSingleResult<TEntity, T> Mapped<TEntity, T>(TEntity entity)
        {
            return new MappedSingleResult<TEntity, T>(Mapper, this, entity);
        }

        protected MappedQueryResult<TEntity, T> Mapped<TEntity, T>(IQueryResult<TEntity> innerResult)
        {
            return new MappedQueryResult<TEntity, T>(Mapper, this, innerResult);
        }

        protected MappedQueryResult<TEntity, T> Mapped<TEntity, T>(IQueryable<TEntity> query)
        {
            return new MappedQueryResult<TEntity, T>(Mapper, this, query);
        }

        protected PagingResult<T> Paged<T>(IOrderedQueryable<T> query)
        {
            return new PagingResult<T>(HttpStatusCode.OK, query, this);
        }

        protected MappedQueryResult<TEntity, T> PagedAndMapped<TEntity, T>(IOrderedQueryable<TEntity> query)
        {
            return Mapped<TEntity, T>(Paged(query));
        }

        protected async Task<SeeOtherAtRouteResult> RedirectToExisting<T, TKey>(IStore<T, TKey> @this, Guid rowGuid, string routeName)
            /* class constraint avoids cast to interface which is incompatible with LINQ to Entities */
            where T : class, IGuidIdentifiable, IIdentifiable
        {
            var existingRows = await @this.GetAsync(
                x => x.TransactionGuid == rowGuid);
            var existing = existingRows.SingleOrDefault();

            if (existing != null)
            {
                return SeeOtherAtRoute(routeName, Route.GetById(existing));
            }

            return null;
        }

        protected SeeOtherAtRouteResult SeeOtherAtRoute(string routeName, object routeValues)
        {
            return new SeeOtherAtRouteResult(routeName, new HttpRouteValueDictionary(routeValues), this);
        }
    }
}