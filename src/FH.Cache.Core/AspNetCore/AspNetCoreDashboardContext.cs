using FH.Cache.Core.Dashboard;
using Hangfire.Dashboard;
using Microsoft.AspNetCore.Http;
using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Antiforgery;

namespace FH.Cache.Core.AspNetCore
{
    public sealed class AspNetCoreDashboardContext : DashboardContext
    {
        public AspNetCoreDashboardContext(
            DashboardOptions options,
            HttpContext httpContext)
            : base(options)
        {
            if (httpContext == null) throw new ArgumentNullException(nameof(httpContext));

            HttpContext = httpContext;
            Request = new AspNetCoreDashboardRequest(httpContext);
            Response = new AspNetCoreDashboardResponse(httpContext);

            if (!options.IgnoreAntiforgeryToken)
            {
                var antiforgery = HttpContext.RequestServices.GetService<IAntiforgery>();
                var tokenSet = antiforgery?.GetAndStoreTokens(HttpContext);

                if (tokenSet != null)
                {
                    AntiforgeryHeader = tokenSet.HeaderName;
                    AntiforgeryToken = tokenSet.RequestToken;
                }
            }
        }

        public HttpContext HttpContext { get; }
    }
}
