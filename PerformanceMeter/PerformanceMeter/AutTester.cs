using System;
using System.Threading;
using log4net;

namespace PerformanceMeter
{
    internal sealed class AutTester
    {
        private ILog log = LogManager.GetLogger(typeof(AutTester));
        private AutLauncher launcher;
        private DateTime exitTime;

        public AutTester()
        {
            launcher = new AutLauncher();
            AutLauncher.Aut.Exited += OnAutCompleted;
        }

        public void StartTest()
        {
            try
            {
                log.Info($"*** Starting '{ArgumentParser.AutPath.Name}' AUT test ***");
                launcher.StartAut();
                launcher.ReadConsoleAsync();
                AutLauncher.Aut.WaitForExit();
            }
            catch(Exception exc)
            {
                log.Fatal($"Application Under Test has been forcibly terminated due to unhandled exception.");
                log.Fatal($"Source: {exc.Source} Message: {exc.Message}");
                if (!AutLauncher.Aut.HasExited)
                    AutLauncher.Aut.Kill();
            }
        }

        public void OnAutCompleted(object sendingProcess, EventArgs output)
        {
            exitTime = AutLauncher.Aut.ExitTime;
            launcher.InputCancellationSource.Cancel();
        }

        public TestResults GenerateResults()
        {
            log.Info($"AUT completed execution in: {AutLauncher.ElapsedTime}");
            return new TestResults(ArgumentParser.AutPath.Name, AutLauncher.ElapsedTime.Elapsed, Convert.ToUInt32(ArgumentParser.ProcessorAffinity.Split(",", StringSplitOptions.RemoveEmptyEntries).Length));
        }
    }
}
