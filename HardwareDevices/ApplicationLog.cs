using NLog;

namespace HWB.Logging
{
    internal static class Log
    {
        static Log()
        {
//#if DEBUG
//            // Setup the logging view for Sentinel - http://sentinel.codeplex.com
//            var sentinalTarget = new NLogViewerTarget()
//            {
//                Name = "sentinal",
//                Address = "udp://127.0.0.1:9999",
//                IncludeNLogData = false
//            };
//            var sentinalRule = new LoggingRule("*", LogLevel.Trace, sentinalTarget);

//            LogManager.Configuration.AddTarget("sentinal", sentinalTarget);
//            LogManager.Configuration.LoggingRules.Add(sentinalRule);


//#endif

            LogManager.ReconfigExistingLoggers();

            Instance = LogManager.GetCurrentClassLogger();
        }

        public static Logger Instance { get; private set; }
    }
}