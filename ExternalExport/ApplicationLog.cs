using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NLog;
using NLog.Config;
using NLog.Targets;



namespace HWB.Logging
{
    internal static class Log
    {
        public static Logger Instance { get; private set; }
        static Log()
        {
#if DEBUG
            // Setup the logging view for Sentinel - http://sentinel.codeplex.com
            var sentinalTarget = new NLogViewerTarget()
            {
                Name = "sentinal",
                Address = "udp://127.0.0.1:9999",
                IncludeNLogData = false
            };
            var sentinalRule = new LoggingRule("*", LogLevel.Trace, sentinalTarget);

            LogManager.Configuration.AddTarget("sentinal", sentinalTarget);
            LogManager.Configuration.LoggingRules.Add(sentinalRule);




#endif

            LogManager.ReconfigExistingLoggers();

            Instance = LogManager.GetCurrentClassLogger();
        }
    }
}