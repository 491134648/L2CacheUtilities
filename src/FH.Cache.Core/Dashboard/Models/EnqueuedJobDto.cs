using System;

namespace FH.Cache.Core.Dashboard.Models
{
    public class EnqueuedJobDto
    {
        public EnqueuedJobDto()
        {
            InEnqueuedState = true;
        }
        public string State { get; set; }
        public DateTime? EnqueuedAt { get; set; }
        public string Key { get; set; }
        public string Value { get; set; }
        public bool InEnqueuedState { get; set; }
    }
}
