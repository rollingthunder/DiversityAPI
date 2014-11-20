namespace DiversityService.API.Filters
{
    using DiversityService.API.Model;
    using DiversityService.API.Services;
    using Ninject;
    using Ninject.Web.Common;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Security.Claims;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Web.Http.Controllers;
    using System.Web.Http.Filters;
    using System.Web.Http.Routing;

    public static class CollectionAPI
    {
        public const string COLLECTION = "collection";
        public const string PROJECT = "project";
    }

    public static class CollectionRequestExtensions
    {
        private const string COLLECTION_CONTEXT_KEY = "coll_context";

        public static IContext GetCollectionContext(this HttpRequestMessage request)
        {
            var requestContext = request.GetOwinContext();
            return requestContext.Get<IContext>(COLLECTION_CONTEXT_KEY);
        }

        public static void SetCollectionContext(this HttpRequestMessage request, IContext ctx)
        {
            var requestContext = request.GetOwinContext();
            requestContext.Set<IContext>(COLLECTION_CONTEXT_KEY, ctx);
        }
    }

    public class CollectionFilter : ActionFilterAttribute
    {
        private readonly IConfigurationService Configuration;
        private readonly IContextFactory ContextFactory;

        public CollectionFilter(
            IConfigurationService config,
            IContextFactory contextFactory
            )
        {
            Configuration = config;
            ContextFactory = contextFactory;
        }

        public override async Task OnActionExecutingAsync(HttpActionContext actionContext, CancellationToken cancellationToken)
        {
            string User, Password;
            InternalCollectionServer Server;

            ExtractCollection(actionContext, out Server);

            if (actionContext.Response != null)
            {
                return;
            }

            if (Server != null)
            {
                ExtractBackendCredentials(actionContext, out User, out Password);

                if (actionContext.Response != null)
                {
                    return;
                }

                var ctx = await ContextFactory.CreateContextAsync(Server, User, Password);

                if (ctx == null)
                {
                    SetErrorResponse(actionContext, HttpStatusCode.Forbidden, "Invalid Collection Credentials");
                    return;
                }

                actionContext.Request.SetCollectionContext(ctx);
            }

            await base.OnActionExecutingAsync(actionContext, cancellationToken);
        }

        private void ExtractBackendCredentials(
            HttpActionContext actionContext,
            out string username,
            out string password)
        {
            username = null;
            password = null;

            var requestContext = actionContext.RequestContext;

            // Check authenticated user and get their backend credentials
            if (requestContext == null)
            {
                SetErrorResponse(actionContext, HttpStatusCode.InternalServerError, "No Request Context Available");
                return;
            }

            var principal = requestContext.Principal;
            if (principal == null
                || principal.Identity == null
                || !principal.Identity.IsAuthenticated)
            {
                SetErrorResponse(actionContext, HttpStatusCode.Unauthorized, "Unauthorized");
                return;
            }

            var identity = principal.Identity as ClaimsIdentity;
            if (identity == null)
            {
                SetErrorResponse(actionContext, HttpStatusCode.InternalServerError, "No claims available for current identity");
                return;
            }

            var backendCredentials = identity.GetBackendCredentialsClaim();
            if (backendCredentials == null)
            {
                SetErrorResponse(actionContext, HttpStatusCode.Forbidden, "No Backend Credentials set");
                return;
            }

            username = backendCredentials.User;
            password = backendCredentials.Password;
        }

        private void ExtractCollection(
            HttpActionContext actionContext,
            out InternalCollectionServer server)
        {
            server = null;

            var request = actionContext.Request;

            var routeData = request.GetRouteData();
            if (routeData == null)
            {
                SetErrorResponse(actionContext, HttpStatusCode.InternalServerError, "No Route Data Available");
                return;
            }

            if (routeData.Values.ContainsKey(CollectionAPI.COLLECTION))
            {
                var collection = routeData.Values[CollectionAPI.COLLECTION].ToString();
                int collectionId;

                if (!int.TryParse(collection, out collectionId))
                {
                    SetErrorResponse(actionContext, HttpStatusCode.BadRequest, "URL Component {collection} must be an integer");
                    return;
                }

                var servers = Configuration.GetCollectionServers();
                server = servers.FirstOrDefault(x => x.Id == collectionId);

                if (server == null)
                {
                    SetErrorResponse(actionContext, HttpStatusCode.BadRequest, "Invalid Collection Server");
                    return;
                }
            }
        }

        private static void SetErrorResponse(HttpActionContext actionContext, HttpStatusCode statusCode, string message)
        {
            actionContext.Response = actionContext.Request.CreateErrorResponse(statusCode, message);
        }
    }
}