using FH.Cache.Core.Configurations;
using FH.Cache.Core.Logging;
using Microsoft.Extensions.Logging;
using System;
using LogLevel = Microsoft.Extensions.Logging.LogLevel;

namespace FH.Cache.Core.AspNetCore
{
    public class AspNetCoreLog : ILog
    {
        private static readonly Func<object, Exception, string> MessageFormatterFunc = MessageFormatter;

        private readonly ILogger _targetLogger;

        public AspNetCoreLog(ILogger targetLogger)
        {
            if (targetLogger == null) throw new ArgumentNullException(nameof(targetLogger));
            _targetLogger = targetLogger;
        }

        public bool Log(Logging.LogLevel logLevel, Func<string> messageFunc, Exception exception = null)
        {
            var targetLogLevel = ToTargetLogLevel(logLevel);

            // When messageFunc is null, Hangfire.Logging
            // just determines is logging enabled.
            if (messageFunc == null)
            {
                return _targetLogger.IsEnabled(targetLogLevel);
            }

            _targetLogger.Log(targetLogLevel, 0, messageFunc(), exception, MessageFormatterFunc);
            return true;
        }

        private static LogLevel ToTargetLogLevel(Logging.LogLevel logLevel)
        {
            switch (logLevel)
            {
                case Logging.LogLevel.Trace:
                    return LogLevel.Trace;
                case Logging.LogLevel.Debug:
                    return LogLevel.Debug;
                case Logging.LogLevel.Info:
                    return LogLevel.Information;
                case Logging.LogLevel.Warn:
                    return LogLevel.Warning;
                case Logging.LogLevel.Error:
                    return LogLevel.Error;
                case Logging.LogLevel.Fatal:
                    return LogLevel.Critical;
            }

            return LogLevel.None;
        }

        private static string MessageFormatter(object state, Exception exception)
        {
            return state.ToString();
        }
    }
}
