using Ninject;
using Ninject.Parameters;
using System;
using System.Collections.Generic;
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
            Kernel = kernel;
            Context = context;

            _Projects = LazyWithContext<IProjectStore>();
        }

        private Lazy<IProjectStore> _Projects;

        public IProjectStore Projects
        {
            get { return _Projects.Value; }
        }

        private Lazy<T> LazyWithContext<T>()
        {
            return new Lazy<T>(() =>
                Kernel.Get<T>(new ConstructorArgument(CONTEXT_ARGUMENT, Context))
                );
        }
    }
}