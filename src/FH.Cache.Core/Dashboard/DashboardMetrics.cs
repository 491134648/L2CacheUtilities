﻿using System;
using System.Collections.Generic;
using System.Linq;
using FH.Cache.Core.Dashboard.Resources;

namespace FH.Cache.Core.Dashboard
{
    public static class DashboardMetrics
    {
        private static readonly Dictionary<string, DashboardMetric> Metrics = new Dictionary<string, DashboardMetric>();

        static DashboardMetrics()
        {
            AddMetric(ServerCount);
            AddMetric(RecurringJobCount);
            AddMetric(EnqueuedCountOrNull);
            AddMetric(FailedCountOrNull);
            AddMetric(EnqueuedAndQueueCount);
            AddMetric(ScheduledCount);
            AddMetric(ProcessingCount);
            AddMetric(SucceededCount);
            AddMetric(FailedCount);
            AddMetric(DeletedCount);
        }

        public static void AddMetric(DashboardMetric metric)
        {
            if (metric == null) throw new ArgumentNullException(nameof(metric));

            lock (Metrics)
            {
                Metrics[metric.Name] = metric;
            }
        }

        public static IEnumerable<DashboardMetric> GetMetrics()
        {
            lock (Metrics)
            {
                return Metrics.Values.ToList();
            }
        }

        public static readonly DashboardMetric ServerCount = new DashboardMetric(
            "servers:count", 
            "Metrics_Servers",
            page => new Metric(page.Statistics.Servers)
            {
                Style = page.Statistics.Servers == 0 ? MetricStyle.Warning : MetricStyle.Default,
                Highlighted = page.Statistics.Servers == 0,
                Title = page.Statistics.Servers == 0
                    ? "No active servers found. Jobs will not be processed."
                    : null
            });

        public static readonly DashboardMetric RecurringJobCount = new DashboardMetric(
            "recurring:count",
            "Metrics_RecurringJobs",
            page => new Metric(page.Statistics.Recurring));

      

        public static readonly DashboardMetric EnqueuedCountOrNull = new DashboardMetric(
            "enqueued:count-or-null",
            "Metrics_EnqueuedCountOrNull",
            page => page.Statistics.Enqueued > 0 || page.Statistics.Failed == 0
                ? new Metric(page.Statistics.Enqueued)
                {
                    Style = page.Statistics.Enqueued > 0 ? MetricStyle.Info : MetricStyle.Default,
                    Highlighted = page.Statistics.Enqueued > 0 && page.Statistics.Failed == 0
                }
                : null);

        public static readonly DashboardMetric FailedCountOrNull = new DashboardMetric(
            "failed:count-or-null",
            "Metrics_FailedJobs",
            page => page.Statistics.Failed > 0
                ? new Metric(page.Statistics.Failed)
                {
                    Style = MetricStyle.Danger,
                    Highlighted = true,
                    Title = string.Format(Strings.Metrics_FailedCountOrNull, page.Statistics.Failed)
                }
                : null);

        public static readonly DashboardMetric EnqueuedAndQueueCount = new DashboardMetric(
            "enqueued-queues:count",
            "Metrics_EnqueuedQueuesCount",
            page => new Metric($"{page.Statistics.Enqueued:N0} / {page.Statistics.Queues:N0}")
            {
                IntValue = page.Statistics.Enqueued,
                Style = page.Statistics.Enqueued > 0 ? MetricStyle.Info : MetricStyle.Default,
                Highlighted = page.Statistics.Enqueued > 0
            });

        public static readonly DashboardMetric ScheduledCount = new DashboardMetric(
            "scheduled:count",
            "Metrics_ScheduledJobs",
            page => new Metric(page.Statistics.Scheduled)
            {
                Style = page.Statistics.Scheduled > 0 ? MetricStyle.Info : MetricStyle.Default
            });

        public static readonly DashboardMetric ProcessingCount = new DashboardMetric(
            "processing:count",
            "Metrics_ProcessingJobs",
            page => new Metric(page.Statistics.Processing)
            {
                Style = page.Statistics.Processing > 0 ? MetricStyle.Warning : MetricStyle.Default
            });

        public static readonly DashboardMetric SucceededCount = new DashboardMetric(
            "succeeded:count",
            "Metrics_SucceededJobs",
            page => new Metric(page.Statistics.Succeeded));

        public static readonly DashboardMetric FailedCount = new DashboardMetric(
            "failed:count",
            "Metrics_FailedJobs",
            page => new Metric(page.Statistics.Failed)
            {
                Style = page.Statistics.Failed > 0 ? MetricStyle.Danger : MetricStyle.Default,
                Highlighted = page.Statistics.Failed > 0
            });

        public static readonly DashboardMetric DeletedCount = new DashboardMetric(
            "deleted:count",
            "Metrics_DeletedJobs",
            page => new Metric(page.Statistics.Deleted));

     
    }
}
