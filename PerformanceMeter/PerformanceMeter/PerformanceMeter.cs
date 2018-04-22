using log4net;
using System;

namespace PerformanceMeter
{
    class PerformanceMeter
    {
        private static ILog log = LogManager.GetLogger(typeof(PerformanceMeter));

        static void Main(string[] args)
        {
            log.InfoFormat("Performance Meter started.");
            ArgumentParser.ParseArguments(ref args);
        }
    }
}
