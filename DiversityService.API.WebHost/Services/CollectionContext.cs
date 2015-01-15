namespace DiversityService.API.Services
{
    using DiversityService.Collection;
    using Ninject;
    using Ninject.Parameters;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Linq;
    using System.Web;

    public class CollectionContext : IContext
    {
        public const string CONTEXT_ARGUMENT = "context";

        private readonly IKernel Kernel;
        private readonly Collection Context;
        private readonly IDictionary<Type, object> StoreCache;

        public CollectionContext(
            IKernel kernel,
            Collection context
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
    }
}