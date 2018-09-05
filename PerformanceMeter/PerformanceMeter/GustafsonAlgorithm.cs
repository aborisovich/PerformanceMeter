using System;
using System.Collections.Generic;
using System.Linq;
using log4net;

namespace PerformanceMeter
{
    class GustafsonAlgorithm
    {
        private ILog log = LogManager.GetLogger(typeof(GustafsonAlgorithm));
        public void AnalizeResults(List<TestResults> testResults)
        {
            log.Info("Calculating AUT alpha factor using Gustafson Law.");
            if (testResults.Count < 2)
            {
                log.Error("Failed to analize test results using Gustafson Law. More than 2 results are required for the comparison.");
                return;
            }
            if (!testResults.Any(result => result.coresCount == 1))
            {
                log.Warn("Test results does not contain single core test runs.");
                log.Error("Failed to analize test results using Gustafson Law. There is no test result from linear execution.");
                return;
            }
            double linearTime = testResults.Where(s => s.coresCount == 1).Select(s => s.executionTime.Ticks).ToList().Average();
            log.Info($"Average AUT time on single core: {TimeSpan.FromTicks((long)linearTime)}");
            List<TestResults> speedupResults = new List<TestResults>(testResults.Where(s => s.coresCount != 1));
            List<double> alphas = new List<double>();
            foreach (var result in speedupResults)
                alphas.Add(CalculateAlpha(linearTime, result));
            double alpha = alphas.Average();
            log.Info($"Alpha factor of '{ArgumentParser.AutPath.Name}': {alpha}");
        }

        private double CalculateAlpha(double linearTime, TestResults speedupResult)
        {
            double speedup = speedupResult.executionTime.Ticks / linearTime;
            return (speedupResult.coresCount - speedup) / (speedupResult.coresCount - 1);
        }
    }
}
