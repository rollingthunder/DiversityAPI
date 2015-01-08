namespace DiversityService.API.Results
{
    using DiversityService.API.Services;
    using System;
    using System.Linq;
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Web.Http;
    using System.Web.Http.Results;

    public class MappedSingleResult<TEntity, T> : IHttpActionResult
    {
        private readonly Lazy<OkNegotiatedContentResult<T>> InnerResult;

        public readonly IMappingService Mapper;
        public readonly ApiController Controller;
        public readonly TEntity Entity;

        public MappedSingleResult(IMappingService mapper, ApiController controller, TEntity entity)
        {
            Mapper = mapper;
            Controller = controller;
            Entity = entity;

            InnerResult = new Lazy<OkNegotiatedContentResult<T>>(() =>
            {
                var mapped = Mapper.Map<T>(Entity);
                return new OkNegotiatedContentResult<T>(mapped, Controller);
            });
        }

        public Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken)
        {
            return InnerResult.Value.ExecuteAsync(cancellationToken);
        }
    }

    public class MappedQueryResult<TEntity, T> : IQueryResult<T>
    {
        private readonly Lazy<OkNegotiatedContentResult<IQueryable<T>>> InnerResult;
        private readonly Lazy<IQueryable<T>> MappedQuery;

        public readonly IMappingService Mapper;
        public readonly ApiController Controller;

        public IQueryable<TEntity> SourceQuery { get; private set; }

        public IQueryable<T> Query
        {
            get { return MappedQuery.Value; }
        }

        public MappedQueryResult(IMappingService mapper, ApiController controller, IQueryable<TEntity> source)
        {
            Mapper = mapper;
            Controller = controller;
            SourceQuery = source;

            MappedQuery = new Lazy<IQueryable<T>>(() => Mapper.Project<TEntity, T>(SourceQuery));

            InnerResult = new Lazy<OkNegotiatedContentResult<IQueryable<T>>>(() =>
            {
                return new OkNegotiatedContentResult<IQueryable<T>>(Query, Controller);
            });
        }

        public Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken)
        {
            return InnerResult.Value.ExecuteAsync(cancellationToken);
        }
    }
}