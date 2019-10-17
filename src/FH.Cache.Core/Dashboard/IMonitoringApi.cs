using FH.Cache.Core.Dashboard.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace FH.Cache.Core.Dashboard
{
    public interface IMonitoringApi
    {
        IList<QueueWithTopEnqueuedJobsDto> Queues();
    }
}
