namespace DiversityService.API.Filters
{
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Security.Claims;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Web.Http.Controllers;
    using System.Web.Http.Filters;
    using DiversityService.API.Model;
    using DiversityService.API.Model.Internal;
    using DiversityService.API.Services;

    public static class CollectionAPI
    {
        public const string ApiPrefix = "api/";
        public const string Collection = "collection";
        public const string CollectionPrefix = "collection/" + CollectionTemplate + "/";
        public const string CollectionTemplate = "{" + Collection + "}";
        public const string Project = "project";
        public const string ProjectPrefix = "project/{" + ProjectTemplate + ":int}/";
        public const string ProjectTemplate = "{" + Project + ":int}";
    }

    public class CollectionContextFilter : ActionFilterAttribute
    {
        private readonly IConfigurationService configuration;
        private readonly IContextFactory contextFactory;

        public CollectionContextFilter(
            IConfigurationService config,
            IContextFactory contextFactory)
        {
            configuration = config;
            this.contextFactory = contextFactory;
        }

        public override async Task OnActionExecutingAsync(HttpActionContext actionContext, CancellationToken cancellationToken)
        {
            AgentInfo agent;
            CollectionServerLogin login;

            IFieldDataContext ctx = null;

            ExtractCollection(actionContext, out login);

            if (actionContext.Response != null)
            {
                return;
            }

            // Only require Identification if we are connecting to a collection 
            if (login != null)
            {
                ClaimsIdentity identity;
                ExtractClaims(actionContext, out identity);

                if (actionContext.Response != null)
                {
                    return;
                }

                // Backend Login Claims 
                {
                    ExtractBackendCredentials(actionContext, identity, login);

                    if (actionContext.Response != null)
                    {
                        return;
                    }

                    ctx = await contextFactory.CreateContextAsync(login);

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

                    actionContext.Request.SetBackendCredentials(login);
                    actionContext.Request.SetCollectionContext(ctx);
                }

                // Agent Claims 
                {
                    ExtractAgentInfo(actionContext, identity, out agent);

                    if (actionContext.Response != null)
                    {
                        return;
                    }

                    if (agent != null)
                    {
                        actionContext.Request.SetAgentInfo(agent);
                    }
                }
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

        private static void SetErrorResponse(HttpActionContext actionContext, HttpStatusCode statusCode, string message)
        {
            actionContext.Response = actionContext.Request.CreateErrorResponse(statusCode, message);
        }

        private void ExtractAgentInfo(
            HttpActionContext ctx,
            ClaimsIdentity identity,
            out AgentInfo info)
        {
            info = null;

            var nameClaim = identity.FindFirst(AgentNameClaim.TYPE);
            var uriClaim = identity.FindFirst(AgentUriClaim.TYPE);

            // AgentInfo is optional, don't fail. 
            if (nameClaim != null && uriClaim != null)
            {
                info = new AgentInfo()
                {
                    Name = nameClaim.Value,
                    Uri = uriClaim.Value
                };
            }
        }

        private void ExtractBackendCredentials(
            HttpActionContext actionContext,
            ClaimsIdentity identity,
            CollectionServerLogin login)
        {
            var backendCredentials = identity.GetBackendCredentialsClaim();
            if (backendCredentials == null)
            {
                SetErrorResponse(actionContext, HttpStatusCode.Forbidden, "No Backend Credentials set");
                return;
            }

            login.User = backendCredentials.User;
            login.Password = backendCredentials.Password;
        }

        private void ExtractClaims(
                                    HttpActionContext actionContext,
            out ClaimsIdentity identity)
        {
            identity = null;

            var requestContext = actionContext.RequestContext;

            // Check authenticated user and get their back end credentials 
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
                SetErrorResponse(actionContext, HttpStatusCode.Unauthorized, "Unauthorized Request");
                return;
            }

            identity = principal.Identity as ClaimsIdentity;
            if (identity == null)
            {
                SetErrorResponse(actionContext, HttpStatusCode.InternalServerError, "No claims available for current identity");
                return;
            }
        }

        private void ExtractCollection(
            HttpActionContext actionContext,
            out CollectionServerLogin login)
        {
            login = null;

            var request = actionContext.Request;

            var routeData = request.GetRouteData();
            if (routeData == null)
            {
                SetErrorResponse(actionContext, HttpStatusCode.InternalServerError, "No Route Data Available");
                return;
            }

            if (routeData.Values.ContainsKey(CollectionAPI.Collection))
            {
                var collection = routeData.Values[CollectionAPI.Collection].ToString();

                int collectionId;

                if (!int.TryParse(collection, out collectionId))
                {
                    SetErrorResponse(actionContext, HttpStatusCode.BadRequest, "URL Component {collection} must be an integer");
                    return;
                }

                var servers = configuration.GetCollectionServers();
                var server = servers.FirstOrDefault(x => x.Id == collectionId);

                if (server == null)
                {
                    SetErrorResponse(actionContext, HttpStatusCode.NotFound, string.Format("Collection Server with id {0} not found", collection));
                    return;
                }

                // Copy over the information from the known server 
                login = new CollectionServerLogin();
                login.Id = server.Id;
                login.Name = server.Name;
                login.Address = server.Address;
                login.Catalog = server.Catalog;
                login.Port = server.Port;
            }
        }

        private async Task ExtractProject(
            HttpActionContext actionContext,
            IFieldDataContext collectionContext)
        {
            var request = actionContext.Request;

            var routeData = request.GetRouteData();
            if (routeData.Values.ContainsKey(CollectionAPI.Project))
            {
                var project = routeData.Values[CollectionAPI.Project].ToString();
                int projectId;

                if (!int.TryParse(project, out projectId))
                {
                    SetErrorResponse(actionContext, HttpStatusCode.BadRequest, "URL Component {project} must be an integer");
                    return;
                }

                var databaseProject = await collectionContext.Projects.GetByIDAsync(projectId);

                if (databaseProject == null)
                {
                    SetErrorResponse(actionContext, HttpStatusCode.NotFound, string.Format("Project with id {0} not found", project));
                    return;
                }

                collectionContext.ProjectId = projectId;
            }
        }
    }
}