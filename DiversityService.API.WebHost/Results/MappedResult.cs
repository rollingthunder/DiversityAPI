namespace DiversityService.API.Results
{
    using AutoMapper;
    using AutoMapper.QueryableExtensions;
    using System;
    using System.Diagnostics.Contracts;
    using System.Linq;
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Web.Http;
    using System.Web.Http.Results;

    public class MappedSingleResult<TEntity, T> : IHttpActionResult, IChainedResult
    {
        private readonly Lazy<OkNegotiatedContentResult<T>> InnerResult;

        public readonly IMappingEngine Mapper;
        public readonly ApiController Controller;
        public readonly TEntity Entity;

        public MappedSingleResult(IMappingEngine mapper, ApiController controller, TEntity entity)
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

        IHttpActionResult IChainedResult.InnerResult
        {
            get { return InnerResult.Value; }
        }
    }

    public class MappedQueryResult<TEntity, T> : IQueryResult<T>, IChainedResult
    {
        private readonly Lazy<IHttpActionResult> InnerResult;
        private readonly Lazy<IQueryable<T>> MappedQuery;

        private readonly IMappingEngine Mapper;
        private readonly ApiController Controller;
        private readonly IQueryable<TEntity> SourceQuery;

        public IQueryable<T> Query
        {
            get { return MappedQuery.Value; }
        }

        private MappedQueryResult(IMappingEngine mapper, ApiController controller)
        {
            Mapper = mapper;
            Controller = controller;
        }

        public MappedQueryResult(IMappingEngine mapper, ApiController controller, IQueryable<TEntity> source)
            : this(mapper, controller)
        {
            SourceQuery = source;

            MappedQuery = new Lazy<IQueryable<T>>(() => SourceQuery.Project(Mapper).To<T>());

            InnerResult = new Lazy<IHttpActionResult>(() =>
            {
                return new OkNegotiatedContentResult<IQueryable<T>>(Query, Controller);
            });
        }

        public MappedQueryResult(IMappingEngine mapper, ApiController controller, IHttpActionResult innerResult)
            : this(mapper, controller)
        {
            Contract.Requires<ArgumentException>(innerResult is IQueryResult<TEntity>);

            var queryresult = innerResult as IQueryResult<TEntity>;
            SourceQuery = queryresult.Query;

            MappedQuery = new Lazy<IQueryable<T>>(() => SourceQuery.Project(Mapper).To<T>());

            InnerResult = new Lazy<IHttpActionResult>(() => innerResult);
        }

        public Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken)
        {
            return InnerResult.Value.ExecuteAsync(cancellationToken);
        }

        IHttpActionResult IChainedResult.InnerResult
        {
            get { return InnerResult.Value; }
        }
    }
}