namespace DiversityService.API.Results
{
    using System;
    using System.Diagnostics.Contracts;
    using System.Linq;
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Web.Http;
    using System.Web.Http.Results;
    using AutoMapper;
    using AutoMapper.QueryableExtensions;

    public class MappedQueryResult<TEntity, T> : IQueryResult<T>, IChainedResult
    {
        private readonly ApiController controller;
        private readonly Lazy<IHttpActionResult> innerResult;
        private readonly Lazy<IQueryable<T>> mappedQuery;

        private readonly IMappingEngine mapper;
        private readonly IQueryable<TEntity> sourceQuery;

        public MappedQueryResult(IMappingEngine mapper, ApiController controller, IQueryable<TEntity> source)
            : this(mapper, controller)
        {
            sourceQuery = source;

            mappedQuery = new Lazy<IQueryable<T>>(() => sourceQuery.Project(this.mapper).To<T>());

            innerResult = new Lazy<IHttpActionResult>(() =>
            {
                return new OkNegotiatedContentResult<IQueryable<T>>(Query, this.controller);
            });
        }

        public MappedQueryResult(IMappingEngine mapper, ApiController controller, IHttpActionResult innerResult)
            : this(mapper, controller)
        {
            Contract.Requires<ArgumentException>(innerResult is IQueryResult<TEntity>);

            var queryresult = innerResult as IQueryResult<TEntity>;
            sourceQuery = queryresult.Query;

            mappedQuery = new Lazy<IQueryable<T>>(() => sourceQuery.Project(this.mapper).To<T>());

            this.innerResult = new Lazy<IHttpActionResult>(() => innerResult);
        }

        private MappedQueryResult(IMappingEngine mapper, ApiController controller)
        {
            this.mapper = mapper;
            this.controller = controller;
        }

        IHttpActionResult IChainedResult.InnerResult
        {
            get { return innerResult.Value; }
        }

        public IQueryable<T> Query
        {
            get { return mappedQuery.Value; }
        }

        public Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken)
        {
            return innerResult.Value.ExecuteAsync(cancellationToken);
        }
    }

    public class MappedSingleResult<TEntity, T> : IHttpActionResult, IChainedResult
    {
        public readonly ApiController Controller;
        public readonly TEntity Entity;
        public readonly IMappingEngine Mapper;
        private readonly Lazy<OkNegotiatedContentResult<T>> innerResult;

        public MappedSingleResult(IMappingEngine mapper, ApiController controller, TEntity entity)
        {
            Mapper = mapper;
            Controller = controller;
            Entity = entity;

            innerResult = new Lazy<OkNegotiatedContentResult<T>>(() =>
            {
                var mapped = Mapper.Map<T>(Entity);
                return new OkNegotiatedContentResult<T>(mapped, Controller);
            });
        }

        IHttpActionResult IChainedResult.InnerResult
        {
            get { return innerResult.Value; }
        }

        public Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken)
        {
            return innerResult.Value.ExecuteAsync(cancellationToken);
        }
    }
}