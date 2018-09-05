using log4net;
using log4net.Config;
using System;
using System.IO;
using System.Diagnostics;
using System.Reflection;

namespace PerformanceMeter
{
    internal class AutLogger
    {
        private ILog log;

        public AutLogger()
        {
            var logRepository = LogManager.CreateRepository("AutRepository");
            XmlConfigurator.Configure(logRepository, new FileInfo(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Settings", "AutLogger.config")));
            log = LogManager.GetLogger(logRepository.Name, typeof(AutLogger));
        }

        public void LogOutput(object sendingProcess, DataReceivedEventArgs output)
        {
            if (!string.IsNullOrEmpty(output.Data))
                log.Info(output.Data);
        }

        public void LogError(object sendingProcess, DataReceivedEventArgs output)
        {
            if (!string.IsNullOrEmpty(output.Data))
                log.Error(output.Data);
        }

        public void LogExit(object sendingProcess, EventArgs output)
        {
            log.Info($"AUT has terminated with exit code: {AutLauncher.Aut.ExitCode}. Exit time: {AutLauncher.Aut.ExitTime}");
        }
    }
}
