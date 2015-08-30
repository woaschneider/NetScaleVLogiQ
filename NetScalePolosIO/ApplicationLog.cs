using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NLog;
using NLog.Config;
using NLog.Targets;



namespace NetScalePolosIO.Logging
{
    internal static class Log
    {
        public static Logger Instance { get; private set; }
        static Log()
        {
#if DEBUG
       




#endif

            LogManager.ReconfigExistingLoggers();

            Instance = LogManager.GetCurrentClassLogger();
        }
    }
}