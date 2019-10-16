using FH.Cache.Core.Configurations;
using FH.Cache.Core.Logging;
using System;
using System.Linq.Expressions;
using System.Reflection;

namespace FH.Cache.Core.LogProviders
{
    public class SerilogLogProvider : ILogProvider
    {
        private readonly Func<string, object> _getLoggerByNameDelegate;
        private readonly SerilogCallbacks _callbacks;
        private static bool _providerIsAvailableOverride = true;

        public SerilogLogProvider()
        {
            if (!IsLoggerAvailable())
            {
                throw new InvalidOperationException("Serilog.Log not found");
            }
            _getLoggerByNameDelegate = GetForContextMethodCall();
            _callbacks = new SerilogCallbacks();
        }

        public static bool ProviderIsAvailableOverride
        {
            get { return _providerIsAvailableOverride; }
            set { _providerIsAvailableOverride = value; }
        }

        public ILog GetLogger(string name)
        {
            return new SerilogLogger(_callbacks, _getLoggerByNameDelegate(name));
        }

        public static bool IsLoggerAvailable()
        {
            return ProviderIsAvailableOverride && GetLogManagerType() != null;
        }

        private static Type GetLogManagerType()
        {
            return Type.GetType("Serilog.Log, Serilog");
        }

        private static Func<string, object> GetForContextMethodCall()
        {
            Type logManagerType = GetLogManagerType();
            MethodInfo method = logManagerType.GetRuntimeMethod("ForContext", new[] { typeof(string), typeof(object), typeof(bool) });
            ParameterExpression propertyNameParam = Expression.Parameter(typeof(string), "propertyName");
            ParameterExpression valueParam = Expression.Parameter(typeof(object), "value");
            ParameterExpression destructureObjectsParam = Expression.Parameter(typeof(bool), "destructureObjects");
            MethodCallExpression methodCall = Expression.Call(null, method, new Expression[]
            {
                propertyNameParam,
                valueParam,
                destructureObjectsParam
            });
            var func = Expression.Lambda<Func<string, object, bool, object>>(methodCall, new[]
            {
                propertyNameParam,
                valueParam,
                destructureObjectsParam
            }).Compile();
            return name => func("SourceContext", name, false);
        }

        internal class SerilogCallbacks
        {
            public readonly object DebugLevel;
            public readonly object ErrorLevel;
            public readonly object FatalLevel;
            public readonly object InformationLevel;
            public readonly object VerboseLevel;
            public readonly object WarningLevel;
            public readonly Func<object, object, bool> IsEnabled;
            public readonly Action<object, object, string> Write;
            public readonly Action<object, object, Exception, string> WriteException;

            public SerilogCallbacks()
            {
                var logEventTypeType = Type.GetType("Serilog.Events.LogEventLevel, Serilog");
                DebugLevel = Enum.Parse(logEventTypeType, "Debug");
                ErrorLevel = Enum.Parse(logEventTypeType, "Error");
                FatalLevel = Enum.Parse(logEventTypeType, "Fatal");
                InformationLevel = Enum.Parse(logEventTypeType, "Information");
                VerboseLevel = Enum.Parse(logEventTypeType, "Verbose");
                WarningLevel = Enum.Parse(logEventTypeType, "Warning");

                // Func<object, object, bool> isEnabled = (logger, level) => { return ((SeriLog.ILogger)logger).IsEnabled(level); }
                var loggerType = Type.GetType("Serilog.ILogger, Serilog");
                MethodInfo isEnabledMethodInfo = loggerType.GetRuntimeMethod("IsEnabled", new Type[] { logEventTypeType });
                ParameterExpression instanceParam = Expression.Parameter(typeof(object));
                UnaryExpression instanceCast = Expression.Convert(instanceParam, loggerType);
                ParameterExpression levelParam = Expression.Parameter(typeof(object));
                UnaryExpression levelCast = Expression.Convert(levelParam, logEventTypeType);
                MethodCallExpression isEnabledMethodCall = Expression.Call(instanceCast, isEnabledMethodInfo, levelCast);
                IsEnabled = Expression.Lambda<Func<object, object, bool>>(isEnabledMethodCall, new[]
                {
                    instanceParam,
                    levelParam
                }).Compile();

                // Action<object, object, string> Write =
                // (logger, level, message) => { ((SeriLog.ILoggerILogger)logger).Write(level, message, new object[]); }
                MethodInfo writeMethodInfo = loggerType.GetRuntimeMethod("Write", new[] { logEventTypeType, typeof(string), typeof(object[]) });
                ParameterExpression messageParam = Expression.Parameter(typeof(string));
                ConstantExpression propertyValuesParam = Expression.Constant(new object[0]);
                MethodCallExpression writeMethodExp = Expression.Call(instanceCast, writeMethodInfo, levelCast, messageParam, propertyValuesParam);
                Write = Expression.Lambda<Action<object, object, string>>(writeMethodExp, new[]
                {
                    instanceParam,
                    levelParam,
                    messageParam
                }).Compile();

                // Action<object, object, string, Exception> WriteException =
                // (logger, level, exception, message) => { ((ILogger)logger).Write(level, exception, message, new object[]); }
                MethodInfo writeExceptionMethodInfo = loggerType.GetRuntimeMethod("Write", new[]
                {
                    logEventTypeType,
                    typeof(Exception),
                    typeof(string),
                    typeof(object[])
                });
                ParameterExpression exceptionParam = Expression.Parameter(typeof(Exception));
                writeMethodExp = Expression.Call(
                    instanceCast,
                    writeExceptionMethodInfo,
                    levelCast,
                    exceptionParam,
                    messageParam,
                    propertyValuesParam);
                WriteException = Expression.Lambda<Action<object, object, Exception, string>>(writeMethodExp, new[]
                {
                    instanceParam,
                    levelParam,
                    exceptionParam,
                    messageParam,
                }).Compile();
            }
        }

        internal class SerilogLogger : ILog
        {
            private readonly SerilogCallbacks _callbacks;
            private readonly object _logger;

            internal SerilogLogger(SerilogCallbacks callbacks, object logger)
            {
                _callbacks = callbacks;
                _logger = logger;
            }

            public bool Log(LogLevel logLevel, Func<string> messageFunc, Exception exception)
            {
                if (messageFunc == null)
                {
                    return _callbacks.IsEnabled(_logger, logLevel);
                }
                if (exception != null)
                {
                    return LogException(logLevel, messageFunc, exception);
                }

                switch (logLevel)
                {
                    case LogLevel.Debug:
                        if (_callbacks.IsEnabled(_logger, _callbacks.DebugLevel))
                        {
                            _callbacks.Write(_logger, _callbacks.DebugLevel, messageFunc());
                            return true;
                        }
                        break;
                    case LogLevel.Info:
                        if (_callbacks.IsEnabled(_logger, _callbacks.InformationLevel))
                        {
                            _callbacks.Write(_logger, _callbacks.InformationLevel, messageFunc());
                            return true;
                        }
                        break;
                    case LogLevel.Warn:
                        if (_callbacks.IsEnabled(_logger, _callbacks.WarningLevel))
                        {
                            _callbacks.Write(_logger, _callbacks.WarningLevel, messageFunc());
                            return true;
                        }
                        break;
                    case LogLevel.Error:
                        if (_callbacks.IsEnabled(_logger, _callbacks.ErrorLevel))
                        {
                            _callbacks.Write(_logger, _callbacks.ErrorLevel, messageFunc());
                            return true;
                        }
                        break;
                    case LogLevel.Fatal:
                        if (_callbacks.IsEnabled(_logger, _callbacks.FatalLevel))
                        {
                            _callbacks.Write(_logger, _callbacks.FatalLevel, messageFunc());
                            return true;
                        }
                        break;
                    default:
                        if (_callbacks.IsEnabled(_logger, _callbacks.VerboseLevel))
                        {
                            _callbacks.Write(_logger, _callbacks.VerboseLevel, messageFunc());
                            return true;
                        }
                        break;
                }
                return false;
            }

            private bool LogException(LogLevel logLevel, Func<string> messageFunc, Exception exception)
            {
                switch (logLevel)
                {
                    case LogLevel.Debug:
                        if (_callbacks.IsEnabled(_logger, _callbacks.DebugLevel))
                        {
                            _callbacks.WriteException(_logger, _callbacks.DebugLevel, exception, messageFunc());
                            return true;
                        }
                        break;
                    case LogLevel.Info:
                        if (_callbacks.IsEnabled(_logger, _callbacks.InformationLevel))
                        {
                            _callbacks.WriteException(_logger, _callbacks.InformationLevel, exception, messageFunc());
                            return true;
                        }
                        break;
                    case LogLevel.Warn:
                        if (_callbacks.IsEnabled(_logger, _callbacks.WarningLevel))
                        {
                            _callbacks.WriteException(_logger, _callbacks.WarningLevel, exception, messageFunc());
                            return true;
                        }
                        break;
                    case LogLevel.Error:
                        if (_callbacks.IsEnabled(_logger, _callbacks.ErrorLevel))
                        {
                            _callbacks.WriteException(_logger, _callbacks.ErrorLevel, exception, messageFunc());
                            return true;
                        }
                        break;
                    case LogLevel.Fatal:
                        if (_callbacks.IsEnabled(_logger, _callbacks.FatalLevel))
                        {
                            _callbacks.WriteException(_logger, _callbacks.FatalLevel, exception, messageFunc());
                            return true;
                        }
                        break;
                    default:
                        if (_callbacks.IsEnabled(_logger, _callbacks.VerboseLevel))
                        {
                            _callbacks.WriteException(_logger, _callbacks.VerboseLevel, exception, messageFunc());
                            return true;
                        }
                        break;
                }
                return false;
            }
        }
    }
}
