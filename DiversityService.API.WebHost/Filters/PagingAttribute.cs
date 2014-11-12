namespace DiversityService.API.Filters
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using System.Web;
    using System.Web.Http.Filters;

    public class PageDescriptor
    {
        public uint skip { get; set; }

        public uint take { get; set; }
    }

    public class PagedResult<T>
    {
        public PageDescriptor Page { get; set; }

        public IEnumerable<T> Contents { get; set; }
    }

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
                    PageDescriptor descriptor;
                    if (actionExecutedContext.Request.RequestUri.TryReadQueryAs<PageDescriptor>(out descriptor))
                    {
                        if (descriptor.take > MaxPage)
                        {
                            descriptor.take = MaxPage;
                        }
                        PageResult(actionExecutedContext, contentValue as IQueryable, descriptor);
                    }
                }
            }
        }

        private void PageResult(HttpActionExecutedContext ctx, IQueryable result, PageDescriptor desc)
        {
            
        }
    }
}