using FH.Cache.Core.AspNetCore;
using FH.Cache.Core.Dashboard;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System;
using Microsoft.Extensions.DependencyInjection;

namespace FH.Cache.Core
{
    public static class AppBuilderExtensions
    {
        public static IApplicationBuilder UseHangfireDashboard(
            this IApplicationBuilder app,
            string pathMatch = "/hangfire",
            DashboardOptions options = null)
        {
            if (app == null) throw new ArgumentNullException(nameof(app));
            if (pathMatch == null) throw new ArgumentNullException(nameof(pathMatch));

            ServiceCollectionExtensions.ThrowIfNotConfigured(app.ApplicationServices);

            var services = app.ApplicationServices;

        
            options = options ?? services.GetService<DashboardOptions>() ?? new DashboardOptions();

            var routes = app.ApplicationServices.GetRequiredService<RouteCollection>();

            app.Map(new PathString(pathMatch), x => x.UseMiddleware<AspNetCoreDashboardMiddleware>( options, routes));

            return app;
        }
    }
}
