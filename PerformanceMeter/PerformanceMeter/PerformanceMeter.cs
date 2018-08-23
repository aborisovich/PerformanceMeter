using log4net;
using log4net.Config;
using System;
using System.IO;
using System.Reflection;
using PerformanceMeter.Settings;

namespace PerformanceMeter
{
    class PerformanceMeter
    {
        private static ILog log = LogManager.GetLogger(typeof(PerformanceMeter));

        static void Main(string[] args)
        {
            try
            {
                ApplicationSettings appSettings = new ApplicationSettings();
                if (!ArgumentParser.ParseArguments(ref args))
                    return;
                log.Info("Performance Meter has started.");
                appSettings.LoadConfig();
                AutTester tester = new AutTester();
                tester.StartTest();
            }
            catch(Exception e)
            {
                log.Fatal($"Performance Meter stopped execution due to error: {e.Message}");
                return;
            }
            log.InfoFormat("Performance Meter completed execution.");
        }
    }
}
