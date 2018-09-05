using System;
using System.Collections.Generic;
using System.Text;

namespace PerformanceMeter
{
    struct TestResults
    {
        public readonly string autName;
        public readonly TimeSpan executionTime;
        public readonly uint coresCount;

        public TestResults(string autName, TimeSpan executionTime, uint coresCount)
        {
            if (string.IsNullOrWhiteSpace(autName))
                throw new ArgumentException("Argument is null or empty", nameof(autName));
            if (executionTime == null)
                throw new ArgumentNullException("Argument is null", nameof(executionTime));
            this.autName = autName;
            this.executionTime = executionTime;
            this.coresCount = coresCount;
        }
    }
}
