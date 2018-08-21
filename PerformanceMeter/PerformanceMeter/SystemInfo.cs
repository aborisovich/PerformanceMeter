using log4net;
using PerformanceMeter.Windows;
using System;
using System.Runtime.InteropServices;

namespace PerformanceMeter
{
    internal class SystemInfo
    {
        private static ILog log = LogManager.GetLogger(typeof(SystemInfo));

        /// <summary>
        /// Provides information about currently used operating system.
        /// </summary>
        public static string OperatingSystem => RuntimeInformation.OSDescription;

        /// <summary>
        /// Provides information regarding platform architecture.
        /// </summary>
        public static string Architecture => RuntimeInformation.OSArchitecture.ToString();

        /// <summary>
        /// Provides total count of processor logical cores.
        /// </summary>
        public static int LogicalCores => Environment.ProcessorCount;

        /// <summary>
        /// Provides total count of processor physical cores.
        /// </summary>
        public static int PhysicalCores => GetCoresCount();
        
        /// <summary>
        /// Returns total number of physical processor cores.
        /// </summary>
        /// <returns>Total number of physical processor cores.</returns>
        private static int GetCoresCount()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                return Processor.HardwareCores.Length;
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                return 0;
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                return 0;
            }
            throw new NotSupportedException($"Not supported operating system: {RuntimeInformation.OSDescription}");
        }

        public override string ToString()
        {
            return  $"Operating system: {OperatingSystem}\n" +
                    $"Architecture: {Architecture}\n" +
                    $"Physical CPU cores: {PhysicalCores}\n" +
                    $"Logical CPU cores: {LogicalCores}";
        }
    }
}
