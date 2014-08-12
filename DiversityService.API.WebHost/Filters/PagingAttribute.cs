namespace DiversityService.API.Filters
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using System.Web;
    using System.Web.Http.Filters;
    using System.Web.Http.OData;
    using System.Web.Http.OData.Query;

    public class EnablePagingAttribute : EnableQueryAttribute
    {
        public int MaxPage
        {
            get { return PageSize; }
            set
            {
                MaxTop = value;
                PageSize = value;
            }
        }

        public EnablePagingAttribute()
        {
            MaxPage = 20;

            AllowedQueryOptions = AllowedQueryOptions.Top | AllowedQueryOptions.Skip;
        }

        public override void OnActionExecuted(HttpActionExecutedContext actionExecutedContext)
        {
            if (actionExecutedContext.Response != null &&
                actionExecutedContext.Response.Content is ObjectContent)
            {
                base.OnActionExecuted(actionExecutedContext);
            }
        }
    }
}