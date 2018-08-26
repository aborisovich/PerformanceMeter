using System;
using System.Diagnostics;
using System.IO;
using log4net;
using PerformanceMeter.Settings;

namespace PerformanceMeter
{
    /// <summary>
    /// Runs Application Under Test on specified hardware and software resources.
    /// </summary>
    internal sealed class AutLauncher
    {
        ILog log = LogManager.GetLogger(typeof(AutLauncher));
        private AutLogger autLogger;

        public Process Aut { get; private set; }

        public ProcessStartInfo AutStartInfo { get; private set; }

        public AutLauncher()
        {
            AutStartInfo = new ProcessStartInfo()
            {
                FileName = ArgumentParser.AutPath.FullName,
                Arguments = ArgumentParser.AutArgs,
                RedirectStandardInput = ApplicationSettings.AutRedirectStandardInput,
                RedirectStandardOutput = ApplicationSettings.AutRedirectStandardOutput,
                RedirectStandardError = ApplicationSettings.AutRedirectStandardError,
                UseShellExecute = !(ApplicationSettings.AutRedirectStandardInput || ApplicationSettings.AutRedirectStandardOutput || ApplicationSettings.AutRedirectStandardError),
                CreateNoWindow = !ApplicationSettings.AutCreateWindow,
            };
        }

        public void StartAut()
        {
            Aut = new Process
            {
                StartInfo = AutStartInfo
            };
            autLogger = new AutLogger(Aut);
            AutSetEvents();
            Aut.Start();
            Aut.ProcessorAffinity = new IntPtr(1);
            Aut.PriorityClass = Enum.Parse<ProcessPriorityClass>(ArgumentParser.AutPriority);
            Aut.BeginOutputReadLine();
            Aut.BeginErrorReadLine();
            log.Info($"AUT has been started. Starting time: {Aut.StartTime}");
            log.Info($"AUT Input arguments: {AutStartInfo.Arguments}");
            log.Info($"AUT Main Thread ID: {Aut.Id}");
            log.Info($"AUT Process priority: {Aut.PriorityClass}");
            log.Info("-- Redirecting user input to AUT --");
            autLogger.WriteInput();
        }

        private void AutSetEvents()
        {
            Aut.EnableRaisingEvents = true;
            Aut.Exited += new EventHandler(autLogger.LogExit);
            
            if (!AutStartInfo.RedirectStandardInput)
                log.Warn("AUT Standard Input redirection is disabled. AUT may require user interaction to receive input.");
            if (AutStartInfo.RedirectStandardOutput)
                Aut.OutputDataReceived += new DataReceivedEventHandler(autLogger.LogOutput);
            else
                log.Warn("AUT Standard Output redirection is disabled. AUT may require user attention.");
            if (AutStartInfo.RedirectStandardError)
                Aut.ErrorDataReceived += new DataReceivedEventHandler(autLogger.LogError);
            else
                log.Warn($"AUT Standard Error redirection is disabled. AUT may cause unhandled by Performance Meter errors.");
        }
    }
}