using System;
using System.Threading.Tasks;
using aspCart.Core.Domain.Statistics;
using aspCart.Core.Interface.Services.Statistics;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace aspCart.Web.Middleware
{
    public class VisitorCounterMiddleware
    {
        private readonly RequestDelegate _next;

        public VisitorCounterMiddleware(
            RequestDelegate next)
        {
            _next = next;
        }

        public Task Invoke(HttpContext context, IVisitorCountService visitorCountService)
        {
            if (context.Session.GetString("visitor_counter") == null || context.Session.GetString("visitor_counter") != "recorder")
            {
                context.Session.SetString("visitor_counter", "recorder");
                var visitorCountEntity = visitorCountService.GetVisitorCountByDate(DateTime.Now);
                if (visitorCountEntity != null)
                {
                    visitorCountService.UpdateVisitorCount(visitorCountEntity);
                }
                else
                {
                    var visitorModel = new VisitorCount
                    {
                        Date = DateTime.Now,
                        ViewCount = 1
                    };
                    visitorCountService.InsertVisitorCount(visitorModel);
                }
            }

            return _next(context);
        }
    }

    public static class VisitorCounterMiddlewareExtentions
    {
        public static IApplicationBuilder UseVisitorCounter(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<VisitorCounterMiddleware>();
        }
    }
}