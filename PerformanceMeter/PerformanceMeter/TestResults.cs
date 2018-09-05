using System;
using System.Collections.Generic;
using System.Text;

namespace PerformanceMeter
{
    struct TestResults
    {
        public readonly TimeSpan executionTime;
        public readonly uint coresCount;

        public TestResults(TimeSpan executionTime, uint coresCount)
        {
            if (executionTime == null)
                throw new ArgumentNullException("Argument is null", nameof(executionTime));
            this.executionTime = executionTime;
            this.coresCount = coresCount;
        }
    }
}
