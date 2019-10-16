using System;
using System.Collections.Generic;
using FH.Cache.Core.Dashboard.Resources;

namespace FH.Cache.Core.Dashboard
{
    public static class JobsSidebarMenu
    {
        public static readonly List<Func<RazorPage, MenuItem>> Items
            = new List<Func<RazorPage, MenuItem>>();

        static JobsSidebarMenu()
        {
            Items.Add(page => new MenuItem(Strings.JobsSidebarMenu_Enqueued, page.Url.LinkToQueues())
            {
                Active = page.RequestPath.StartsWith("/jobs/enqueued"),
                Metric = DashboardMetrics.EnqueuedAndQueueCount
            });

            Items.Add(page => new MenuItem(Strings.JobsSidebarMenu_Scheduled, page.Url.To("/jobs/scheduled"))
            {
                Active = page.RequestPath.StartsWith("/jobs/scheduled"),
                Metric = DashboardMetrics.ScheduledCount
            });

            Items.Add(page => new MenuItem(Strings.JobsSidebarMenu_Processing, page.Url.To("/jobs/processing"))
            {
                Active = page.RequestPath.StartsWith("/jobs/processing"),
                Metric = DashboardMetrics.ProcessingCount
            });

            Items.Add(page => new MenuItem(Strings.JobsSidebarMenu_Succeeded, page.Url.To("/jobs/succeeded"))
            {
                Active = page.RequestPath.StartsWith("/jobs/succeeded"),
                Metric = DashboardMetrics.SucceededCount
            });

            Items.Add(page => new MenuItem(Strings.JobsSidebarMenu_Failed, page.Url.To("/jobs/failed"))
            {
                Active = page.RequestPath.StartsWith("/jobs/failed"),
                Metric = DashboardMetrics.FailedCount
            });

            Items.Add(page => new MenuItem(Strings.JobsSidebarMenu_Deleted, page.Url.To("/jobs/deleted"))
            {
                Active = page.RequestPath.StartsWith("/jobs/deleted"),
                Metric = DashboardMetrics.DeletedCount
            });

           
        }
    }
}