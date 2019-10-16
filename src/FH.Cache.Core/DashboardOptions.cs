using FH.Cache.Core.Dashboard;
using System;
using System.Collections.Generic;

namespace FH.Cache.Core
{
    public class DashboardOptions
    {
        public DashboardOptions()
        {
            AppPath = "/";
            Authorization = new[] { new LocalRequestsOnlyAuthorizationFilter() };
            IsReadOnlyFunc = _ => false;
            StatsPollingInterval = 2000;
            DisplayStorageConnectionString = true;
            DisplayNameFunc = null;
        }

        /// <summary>
        /// The path for the Back To Site link. Set to <see langword="null" /> in order to hide the Back To Site link.
        /// </summary>
        public string AppPath { get; set; }

        public IEnumerable<IDashboardAuthorizationFilter> Authorization { get; set; }

        public Func<DashboardContext, bool> IsReadOnlyFunc { get; set; }

        /// <summary>
        /// The interval the /stats endpoint should be polled with.
        /// </summary>
        public int StatsPollingInterval { get; set; }

        public bool DisplayStorageConnectionString { get; set; }
        /// <summary>
        /// Display name provider for jobs
        /// </summary>
        public Func<DashboardContext, string, string> DisplayNameFunc { get; set; }
        ///// <summary>
        ///// Display name provider for jobs
        ///// </summary>
        //public Func<DashboardContext, Job, string> DisplayNameFunc { get; set; }

        public bool IgnoreAntiforgeryToken { get; set; }
    }
}
