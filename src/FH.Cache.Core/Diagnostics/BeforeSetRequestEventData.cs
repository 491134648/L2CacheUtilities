using System;
using System.Collections.Generic;

namespace FH.Cache.Core.Diagnostics
{
    public class BeforeSetRequestEventData : EventData
    {
        public BeforeSetRequestEventData(string cacheType, string name, string operation, IDictionary<string, object> dict, System.TimeSpan expiration)
            : base(cacheType, name, operation)
        {
            this.Dict = dict;
            this.Expiration = expiration;
        }

        public IDictionary<string, object> Dict { get; set; }

        public TimeSpan Expiration { get; set; }
    }
}
