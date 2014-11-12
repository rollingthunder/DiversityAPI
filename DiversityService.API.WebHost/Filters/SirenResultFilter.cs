namespace DiversityService.API.Filters
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Linq;
    using System.Net.Http;
    using System.Web;
    using System.Web.Http.Filters;
    using WebApiContrib.Formatting.Siren.Client;

    public interface ISirenProvider
    {
        bool CanTranslate(Type type);

        ISirenEntity Translate(object obj);
    }

    public class SirenResultAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuted(HttpActionExecutedContext actionExecutedContext)
        {
            Contract.Requires<ArgumentNullException>(actionExecutedContext != null, "actionExecutedContext");

            var response = actionExecutedContext.Response;
            Contract.Requires<ArgumentException>(response != null, "The argument MUST contain a Response Object.");

            var responseContent = actionExecutedContext.Response.Content as ObjectContent;
            if (responseContent != null && responseContent.Value != null)
            {
                var valueType = responseContent.Value.GetType();

                var providers = GetProviders(actionExecutedContext);

                var matchingProvider = (from provider in providers
                                        where provider.CanTranslate(valueType)
                                        select provider).FirstOrDefault();

                if (matchingProvider != null)
                {
                    responseContent.Value = matchingProvider.Translate(responseContent.Value);
                }
            }
        }

        private static IEnumerable<ISirenProvider> GetProviders(HttpActionExecutedContext actionExecutedContext)
        {
            Contract.Requires<ArgumentException>(actionExecutedContext.ActionContext != null, "The ActionContext may not be NULL");
            Contract.Requires<ArgumentException>(actionExecutedContext.ActionContext.ControllerContext != null, "The ControllerContext may not be NULL");
            Contract.Requires<ArgumentException>(actionExecutedContext.ActionContext.ControllerContext.Configuration != null, "The Configuration may not be NULL");
            Contract.Requires<ArgumentException>(actionExecutedContext.ActionContext.ControllerContext.Configuration.DependencyResolver != null, "The DependencyResolver may not be NULL");

            var actionContext = actionExecutedContext.ActionContext;
            var controllerContext = actionContext.ControllerContext;
            var configuration = controllerContext.Configuration;
            var dependencyResolver = configuration.DependencyResolver;
            var providers = dependencyResolver.GetServices(typeof(ISirenProvider)) ?? Enumerable.Empty<object>();

            return providers.Cast<ISirenProvider>();
        }
    }
}