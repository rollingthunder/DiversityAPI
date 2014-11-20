namespace DiversityService.API.Test
{
    using Microsoft.Owin;
    using System.Net.Http;
    using System.Security.Claims;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Web.Http.Controllers;
    using System.Web.Http.Filters;

    public class FilterTestBase<T> : TestBase where T : ActionFilterAttribute
    {
        protected HttpRequestMessage Request = new HttpRequestMessage();
        protected HttpRequestContext RequestContext = new HttpRequestContext();
        protected HttpControllerContext ControllerContext;
        protected HttpActionContext ActionContext;

        protected T Filter;

        protected Task InvokeFilter()
        {
            return Filter.OnActionExecutingAsync(ActionContext, CancellationToken.None);
        }

        protected bool ActionCalled()
        {
            return ActionContext.Response == null;
        }

        protected void InitializeActionContext()
        {
            Request.SetOwinContext(new OwinContext());
            RequestContext.Principal = new ClaimsPrincipal(new ClaimsIdentity("test"));
            ControllerContext = new HttpControllerContext() { Request = Request, RequestContext = RequestContext };
            ActionContext = new HttpActionContext { ControllerContext = ControllerContext };
        }
    }
}