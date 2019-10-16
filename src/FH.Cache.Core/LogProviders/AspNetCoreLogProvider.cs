using FH.Cache.Core.AspNetCore;
using FH.Cache.Core.Logging;
using Microsoft.Extensions.Logging;
using System;

namespace FH.Cache.Core.LogProviders
{
    public class AspNetCoreLogProvider : ILogProvider
    {
        private readonly ILoggerFactory _loggerFactory;

        public AspNetCoreLogProvider(ILoggerFactory loggerFactory)
        {
            if (loggerFactory == null) throw new ArgumentNullException(nameof(loggerFactory));
            _loggerFactory = loggerFactory;
        }

        public ILog GetLogger(string name)
        {
            return new AspNetCoreLog(_loggerFactory.CreateLogger(name));
        }
    }
}
