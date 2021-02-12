using System;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ForumAPI.Filters
{
    public sealed class RequestCheckFilter : IResourceFilter
    {
        public void OnResourceExecuted(ResourceExecutedContext context)
        {

        }

        public void OnResourceExecuting(ResourceExecutingContext context)
        {
            var containsCookie = context.HttpContext.Request.Cookies.ContainsKey("last visit");
            
            if (!containsCookie)
            {
                context.HttpContext.Response.Cookies.Append("last_visit", DateTime.Now.ToShortDateString());
            }
        }
    }
}