using System;
using log4net;

namespace PerformanceMeter
{
    internal sealed class AutTester
    {
        private ILog log = LogManager.GetLogger(typeof(AutTester));
        private AutLauncher launcher;

        public AutTester()
        {
            launcher = new AutLauncher();
        }

        public void StartTest()
        {
            try
            {
                log.Info($"*** Starting '{ArgumentParser.AutPath.Name}' AUT test ***");
                launcher.StartAut();
            }
            catch(Exception exc)
            {
                log.Fatal($"Application Under Test has been forcibly terminated due to unhandled exception.");
                log.Fatal($"Source: {exc.Source} Message: {exc.Message}");
                if (!launcher.Aut.HasExited)
                    launcher.Aut.Kill();
            }
        }
    }
}
