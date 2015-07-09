namespace DiversityService.API.Services
{
    using DiversityService.DB.Collection;
    using Ninject;
    using Ninject.Parameters;
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Diagnostics.Contracts;
    using System.Linq;
    using System.Web;

    internal class CollectionTransaction : ITransaction
    {
        private readonly DbContextTransaction Inner;

        private bool _IsClosed;

        public bool IsClosed
        {
            get
            {
                return _IsClosed;
            }
        }

        public CollectionTransaction(DbContextTransaction inner)
        {
            Inner = inner;
            _IsClosed = false;
        }

        public void Commit()
        {
            Inner.Commit();
            _IsClosed = true;
        }

        public void Dispose()
        {
            Inner.Rollback();
            Inner.Dispose();
            _IsClosed = true;
        }
    }

    public class CollectionContext : IContext
    {
        public const string CONTEXT_ARGUMENT = "context";

        private readonly IKernel Kernel;
        private readonly DiversityCollection Context;
        private readonly IDictionary<Type, object> StoreCache;

        private CollectionTransaction CurrentTransaction;

        public CollectionContext(
            IKernel kernel,
            DiversityCollection context
            )
        {
            Contract.Requires<ArgumentNullException>(kernel != null);
            Contract.Requires<ArgumentNullException>(context != null);

            Kernel = kernel;
            Context = context;
            StoreCache = new Dictionary<Type, object>();
        }

        public int? ProjectId
        {
            get
            {
                return Context.ProjectId;
            }
            set
            {
                Context.ProjectId = value;
            }
        }

        public IProjectStore Projects
        {
            get { return LazyWithContext<IProjectStore>(); }
        }

        public IStore<Event, int> Events
        {
            get { return LazyWithContext<IStore<Event, int>>(); }
        }

        public IStore<EventSeries, int> Series
        {
            get { return LazyWithContext<IStore<EventSeries, int>>(); }
        }

        public IStore<Specimen, int> Specimen
        {
            get { return LazyWithContext<IStore<Specimen, int>>(); }
        }

        public IStore<IdentificationUnit, IdentificationUnitKey> IdentificationUnits
        {
            get { return LazyWithContext<IStore<IdentificationUnit, IdentificationUnitKey>>(); }
        }

        public IStore<Identification, IdentificationKey> Identifications
        {
            get { return LazyWithContext<IStore<Identification, IdentificationKey>>(); }
        }

        public IStore<IdentificationUnitGeoAnalysis, IdentificationGeoKey> IdentificationGeoAnalyses
        {
            get { return LazyWithContext<IStore<IdentificationUnitGeoAnalysis, IdentificationGeoKey>>(); }
        }

        private T LazyWithContext<T>()
            where T : class
        {
            var key = typeof(T);
            object value;

            if (!StoreCache.TryGetValue(key, out value))
            {
                value = Kernel.Get<T>(new ConstructorArgument(CONTEXT_ARGUMENT, Context));
                StoreCache.Add(key, value);
            }

            return (T)value;
        }

        public void Dispose()
        {
            Context.Dispose();
        }

        public ITransaction BeginTransaction()
        {
            if (CurrentTransaction != null && !CurrentTransaction.IsClosed)
            {
                throw new InvalidOperationException("Cannot open transaction, while there is still an open one");
            }

            CurrentTransaction = new CollectionTransaction(Context.Database.BeginTransaction());

            return CurrentTransaction;
        }
    }
}