using PerformanceMeter.Settings;
using System;
using System.Collections.Generic;
using System.Text;

namespace PerformanceMeter
{
    static class CoreFunctions
    {
        public static void PerformAutTest()
        {
            AutTester tester = new AutTester();
            tester.StartTest();
            TestResults testResults = tester.GenerateResults();
            new ResultsXmlHandler().WriteResults(ApplicationSettings.ResultsFile, testResults);
        }

        public static void AnalizeTestResults()
        {
            List<TestResults> testsResults = new ResultsXmlHandler().ReadResults(ArgumentParser.ResultsFile);
            new GustafsonAlgorithm().AnalizeResults(testsResults);
        }
    }
}
