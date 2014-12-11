namespace DiversityService.API.Filters
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using System.Web;
    using System.Web.Http.Filters;

    public class EnablePagingAttribute : ActionFilterAttribute
    {
        private uint _MaxPage = 20;

        public uint MaxPage
        {
            get { return _MaxPage; }
            set { _MaxPage = value; }
        }

        public override void OnActionExecuted(HttpActionExecutedContext actionExecutedContext)
        {
            if (actionExecutedContext.Response != null &&
                actionExecutedContext.Response.Content is ObjectContent)
            {
                var contentValue = (actionExecutedContext.Response.Content as ObjectContent).Value;
                if (contentValue is IQueryable)
                {
                }
            }
        }
    }
}