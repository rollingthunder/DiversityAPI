namespace DiversityService.API.Filters
{
    using DiversityService.API.Model;
    using DiversityService.API.Services;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Security.Claims;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Web.Http.Controllers;
    using System.Web.Http.Filters;

    public static class CollectionAPI
    {
        public const string COLLECTION = "collection";
        public const string COLLECTION_TEMPLATE = "{" + COLLECTION + ":int}";
        public const string PROJECT = "project";
        public const string PROJECT_TEMPLATE = "{" + PROJECT + ":int}";
        public const string API_PREFIX = "api/";
        public const string COLLECTION_PREFIX = "collection/" + COLLECTION_TEMPLATE + "/";
        public const string PROJECT_PREFIX = "project/{" + PROJECT_TEMPLATE + ":int}/";
    }

    public class CollectionContextFilter : ActionFilterAttribute
    {
        private readonly IConfigurationService Configuration;
        private readonly IContextFactory ContextFactory;

        public CollectionContextFilter(
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

            IContext ctx;

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

                ctx = await ContextFactory.CreateContextAsync(Server, User, Password);

                if (ctx == null)
                {
                    SetErrorResponse(actionContext, HttpStatusCode.Forbidden, "Invalid Collection Credentials");
                    return;
                }

                await ExtractProject(actionContext, ctx);

                if (actionContext.Response != null)
                {
                    return;
                }

                actionContext.Request.SetCollectionContext(ctx);
            }

            try
            {
                await base.OnActionExecutingAsync(actionContext, cancellationToken);
            }
            catch
            {
                if (ctx != null)
                {
                    ctx.Dispose();
                }

                throw;
            }
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
                    SetErrorResponse(actionContext, HttpStatusCode.NotFound, string.Format("Collection Server with id {0} not found", collection));
                    return;
                }
            }
        }

        private async Task ExtractProject(
            HttpActionContext actionContext,
            IContext collectionContext)
        {
            var request = actionContext.Request;

            var routeData = request.GetRouteData();
            if (routeData.Values.ContainsKey(CollectionAPI.PROJECT))
            {
                var project = routeData.Values[CollectionAPI.PROJECT].ToString();
                int projectId;

                if (!int.TryParse(project, out projectId))
                {
                    SetErrorResponse(actionContext, HttpStatusCode.BadRequest, "URL Component {project} must be an integer");
                    return;
                }

                var dbProject = await collectionContext.Projects.GetByIDAsync(projectId);

                if (dbProject == null)
                {
                    SetErrorResponse(actionContext, HttpStatusCode.NotFound, string.Format("Project with id {0} not found", project));
                    return;
                }

                collectionContext.ProjectId = projectId;
            }
        }

        private static void SetErrorResponse(HttpActionContext actionContext, HttpStatusCode statusCode, string message)
        {
            actionContext.Response = actionContext.Request.CreateErrorResponse(statusCode, message);
        }
    }
}