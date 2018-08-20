using log4net;
using log4net.Config;
using System;
using System.IO;
using System.Reflection;

namespace PerformanceMeter
{
    class PerformanceMeter
    {
        private static ILog log = LogManager.GetLogger(typeof(PerformanceMeter));

        static void Main(string[] args)
        {
            try
            {
                var logRepository = LogManager.GetRepository(Assembly.GetEntryAssembly());
                XmlConfigurator.Configure(logRepository, new FileInfo(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Log.config")));
                log.InfoFormat("Performance Meter started.");
                ArgumentParser.ParseArguments(ref args);
            }
            catch(Exception e)
            {
                log.Error($"Performance Meter stopped execution due to error: {e.Message}");
                return;
            }
            log.InfoFormat("Performance Meter completed execution.");
        }
    }
}
