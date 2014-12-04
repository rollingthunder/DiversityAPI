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
            _Events = LazyWithContext<IEventStore>();
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

        private Lazy<IEventStore> _Events;

        public IEventStore Events
        {
            get { return _Events.Value; }
        }

        private Lazy<T> LazyWithContext<T>()
        {
            return new Lazy<T>(() =>
                Kernel.Get<T>(new ConstructorArgument(CONTEXT_ARGUMENT, Context))
                );
        }
    }
}