using System.Collections.Generic;

namespace FH.Cache.Core.Dashboard.Models
{
    public class QueueWithTopEnqueuedJobsDto
    {
        public string Name { get; set; }
        public long Length { get; set; }
        public long? Fetched { get; set; }
        public List<EnqueuedJobDto> FirstJobs { get; set; }
    }
}
