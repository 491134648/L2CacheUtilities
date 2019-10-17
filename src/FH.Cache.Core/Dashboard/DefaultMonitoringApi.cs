using System;
using System.Collections.Generic;
using System.Text;
using FH.Cache.Core.Dashboard.Models;

namespace FH.Cache.Core.Dashboard
{
    public class DefaultMonitoringApi : IMonitoringApi
    {
        public IList<QueueWithTopEnqueuedJobsDto> Queues()
        {
            return new List<QueueWithTopEnqueuedJobsDto>()
            {
                new QueueWithTopEnqueuedJobsDto
                {
                    Name="测试"
                }
            };
        }
    }
}
