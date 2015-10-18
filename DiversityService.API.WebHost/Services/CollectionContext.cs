namespace DiversityService.API.Services
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Diagnostics.Contracts;
    using DiversityService.DB.Collection;
    using Ninject;
    using Ninject.Parameters;

    public class CollectionContext : IFieldDataContext
    {
        public const string ContextArgument = "context";

        private readonly DiversityCollection context;
        private readonly IKernel kernel;
        private readonly IDictionary<Type, object> storeCache;

        private CollectionTransaction currentTransaction;

        public CollectionContext(
            IKernel kernel,
            DiversityCollection context)
        {
            Contract.Requires<ArgumentNullException>(kernel != null);
            Contract.Requires<ArgumentNullException>(context != null);

            this.kernel = kernel;
            this.context = context;
            storeCache = new Dictionary<Type, object>();
        }

        public IStore<Event, int> Events
        {
            get { return LazyWithContext<IStore<Event, int>>(); }
        }

        public IStore<IdentificationUnitGeoAnalysis, IdentificationGeoKey> IdentificationGeoAnalyses
        {
            get { return LazyWithContext<IStore<IdentificationUnitGeoAnalysis, IdentificationGeoKey>>(); }
        }

        public IStore<Identification, IdentificationKey> Identifications
        {
            get { return LazyWithContext<IStore<Identification, IdentificationKey>>(); }
        }

        public IStore<IdentificationUnit, IdentificationUnitKey> IdentificationUnits
        {
            get { return LazyWithContext<IStore<IdentificationUnit, IdentificationUnitKey>>(); }
        }

        public int? ProjectId
        {
            get
            {
                return context.ProjectId;
            }

            set
            {
                context.ProjectId = value;
            }
        }

        public IProjectStore Projects
        {
            get { return LazyWithContext<IProjectStore>(); }
        }

        public IStore<EventSeries, int> Series
        {
            get { return LazyWithContext<IStore<EventSeries, int>>(); }
        }

        public IStore<Specimen, int> Specimen
        {
            get { return LazyWithContext<IStore<Specimen, int>>(); }
        }

        public ITransaction BeginTransaction()
        {
            if (currentTransaction != null && !currentTransaction.IsClosed)
            {
                throw new InvalidOperationException("Cannot open transaction, while there is still an open one");
            }

            currentTransaction = new CollectionTransaction(context.Database.BeginTransaction());

            return currentTransaction;
        }

        public void Dispose()
        {
            context.Dispose();
        }

        private T LazyWithContext<T>()
                            where T : class
        {
            var key = typeof(T);
            object value;

            if (!storeCache.TryGetValue(key, out value))
            {
                value = kernel.Get<T>(new ConstructorArgument(ContextArgument, context));
                storeCache.Add(key, value);
            }

            return (T)value;
        }
    }

    internal class CollectionTransaction : ITransaction
    {
        private readonly DbContextTransaction inner;

        private bool isClosed;

        public CollectionTransaction(DbContextTransaction inner)
        {
            this.inner = inner;
            isClosed = false;
        }

        public bool IsClosed
        {
            get
            {
                return isClosed;
            }
        }

        public void Commit()
        {
            inner.Commit();
            isClosed = true;
        }

        public void Dispose()
        {
            inner.Rollback();
            inner.Dispose();
            isClosed = true;
        }
    }
}