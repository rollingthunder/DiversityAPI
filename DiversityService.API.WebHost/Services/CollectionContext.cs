using Ninject;
using Ninject.Parameters;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Web;

namespace DiversityService.API.Services
{
    public class CollectionContext : IContext
    {
        public const string CONTEXT_ARGUMENT = "context";

        private readonly IKernel Kernel;
        private readonly Collection.Collection Context;

        public CollectionContext(
            IKernel kernel,
            Collection.Collection context
            )
        {
            Contract.Requires<ArgumentNullException>(kernel != null);
            Contract.Requires<ArgumentNullException>(context != null);

            Kernel = kernel;
            Context = context;

            _Projects = LazyWithContext<IProjectStore>();
            _Events = LazyWithContext<IStore<Collection.Event, int>>();
            _Series = LazyWithContext<IStore<Collection.EventSeries, int>>();
            _Specimen = LazyWithContext<IStore<Collection.Specimen, int>>();
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

        private Lazy<IProjectStore> _Projects;

        public IProjectStore Projects
        {
            get { return _Projects.Value; }
        }

        private Lazy<IStore<Collection.Event, int>> _Events;

        public IStore<Collection.Event, int> Events
        {
            get { return _Events.Value; }
        }

        private Lazy<IStore<Collection.EventSeries, int>> _Series;

        public IStore<Collection.EventSeries, int> Series
        {
            get { return _Series.Value; }
        }

        private Lazy<IStore<Collection.Specimen, int>> _Specimen;

        public IStore<Collection.Specimen, int> Specimen
        {
            get { return _Specimen.Value; }
        }

        private Lazy<T> LazyWithContext<T>()
        {
            return new Lazy<T>(() =>
                Kernel.Get<T>(new ConstructorArgument(CONTEXT_ARGUMENT, Context))
                );
        }
    }
}