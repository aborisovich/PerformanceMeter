using log4net;
using PerformanceMeter.Attributes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Linq;
using System.Runtime.CompilerServices;
using System.ComponentModel;
using System.Diagnostics;

[assembly: InternalsVisibleTo("PerformanceMeterUT")]
namespace PerformanceMeter
{
    /// <summary>
    /// This class parses input arguments of <see cref="PerformanceMeter"/> program.
    /// </summary>
    internal static class ArgumentParser
    {
        private static FileInfo autPath;
        private static ProcessPriorityClass autPriority = ProcessPriorityClass.Normal;
        private static List<uint> processorAffinity;
        private static FileInfo outputFile;
        private static ILog log = LogManager.GetLogger(typeof(ArgumentParser));

        /// <summary>
        /// Path to the Application Under Test.
        /// </summary>
        /// <exception cref="FileNotFoundException">Thrown when Application Under Test does not exist under provided <see cref="Path"/>.</exception>
        /// <exception cref="FileLoadException">Thrown when Application Under Test has invalid attributes.</exception>
        [InputArgument("Path to the Application Under Test.", "-a")]
        internal static FileInfo AutPath
        {
            get { return autPath; }
            private set
            {
                if (!value.Exists)
                    throw new FileNotFoundException("Application Under Test not found under provided path", value.FullName);
                switch (value.Attributes)
                {
                    case FileAttributes.Compressed:
                        throw new FileLoadException("Application Under Test is a compressed file and can not be executed", value.FullName);
                    case FileAttributes.Directory:
                        throw new FileLoadException("Provided path for Application Under Test is a directory", value.FullName);
                    case FileAttributes.Offline:
                    case FileAttributes.System:
                    case FileAttributes.ReparsePoint:
                        throw new FileLoadException("Application Under Test can not be executed due to invalid file properties", value.FullName);
                }
                autPath = value;
            }
        }

        /// <summary>
        /// Application Under Test input arguments.
        /// </summary>
        [InputArgument("Application Under Test input arguments.", "-i")]
        internal static string AutArgs { get; private set; }

        [InputArgument("Application Under Test process priority.", "-p")]
        internal static string AutPriority
        {
            get { return autPriority.ToString(); }
            set
            {
                if (!Enum.TryParse(value, out autPriority))
                    throw new ArgumentException("Invalid priority name value", nameof(AutPriority));
            }
        }

        /// <summary>
        /// Index numbers of the logical processor threads that can be used by Application Under Test.
        /// </summary>
        /// <exception cref="ArgumentNullException">Thrown on attempt to assign null.</exception>
        /// <exception cref="ArgumentException">Thrown on attempt to assign empty list.</exception>
        [InputArgument("Logical processor numbers that can be used by the Application Under Test (separated by ',').", "-c")]
        internal static string ProcessorAffinity
        {
            get
            {
                if (processorAffinity != null && processorAffinity.Count > 0)
                    return string.Join(",", processorAffinity);
                return null;
            }
            private set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException("Argument is null or empty", nameof(ProcessorAffinity));
                var processors = new List<uint>(value.Split(",", StringSplitOptions.RemoveEmptyEntries).Select(s => uint.Parse(s)));
                if (processors.Contains(0))
                    throw new ArgumentException("Value can not be set to 0", nameof(ProcessorAffinity));
                processorAffinity = processors;
            }   
        }

        /// <summary>
        /// Path to the output file containing AUT performance test report and analisys.
        /// </summary>
        /// <exception cref="DirectoryNotFoundException">Thrown when directory specified for writing output file does not exist.</exception>
        [InputArgument("Path to the file containing report regarding AUT execution.", "-o")]
        internal static FileInfo OutputFile
        {
            get { return outputFile; }
            private set
            {
                if (!value.Directory.Exists)
                    throw new DirectoryNotFoundException("Directory for the output file does not exist");
                outputFile = value;
            }
        }


        /// <summary>
        /// Parses input arguments and sets all <see cref="ArgumentParser"/> property values.
        /// </summary>
        /// <param name="args"><see cref="global::PerformanceMeter"/> input arguments.</param>
        /// <returns>True when arguments were parsed successfully, false when none provided.</returns>
        public static bool ParseArguments(ref string[] args)
        {
            if (args == null || args.Length == 0)
            {
                DisplayHelp(GetArgumentProperties());
                return false;
            }
            log.Debug($"Input arguments: {string.Join(" ", args)}");
            try
            {
                SetArgumentProperties(ref args, GetArgumentProperties());
            }
            catch(IndexOutOfRangeException)
            {
                log.Error("Parsing input arguments failed: Some input arguments does not have a value provided!");
                throw;
            }
            catch(TargetInvocationException e)
            {
                log.Error($"Parsing input arguments failed: {e.InnerException.Message}");
                throw;
            }
            catch(Exception e)
            {
                log.Fatal("Fatal error during parsing input arguments!");
                log.Fatal($"Message: {e.Message}");
                throw;
            }
            return true;
        }

        /// <summary>
        /// Retrieves list of processor affinity numbers.
        /// </summary>
        /// <returns></returns>
        public static List<uint> GetProcessorAffinity()
        {
            if (processorAffinity == null || processorAffinity.Count == 0)
                return null;
            return processorAffinity;
        }

        private static void SetArgumentProperties(ref string[] args, List<PropertyInfo> properties)
        {
            foreach (var property in properties)
            {
                InputArgumentAttribute propertyAttribute = property.GetCustomAttribute(typeof(InputArgumentAttribute)) as InputArgumentAttribute;
                for (int i = 0; i < args.Length; i++)
                {
                    if ((propertyAttribute.Texts as List<string>).Contains(args[i]))
                    {
                        TypeConverter converter = TypeDescriptor.GetConverter(property.GetType());
                        if (converter.CanConvertFrom(typeof(string)))
                            property.SetValue(null, converter.ConvertFrom(args[i + 1]));
                        else
                        {
                            ConstructorInfo constructor = property.PropertyType.GetConstructors()
                                .Where(c => c.GetParameters().Length == 1 && c.GetParameters().First().ParameterType == typeof(string)).FirstOrDefault();
                            dynamic instance = (constructor == null) ? args[i + 1] : constructor.Invoke(new object[] { args[i + 1] });

                            property.SetValue(null, instance);
                        }
                            
                    }
                           
                }
            }
        }

        private static void DisplayHelp(IEnumerable<PropertyInfo> argumentProperties)
        {
            string helpText = "Help:\n";
            foreach(var property in argumentProperties)
            {
                var attribute = property.GetCustomAttribute(typeof(InputArgumentAttribute)) as InputArgumentAttribute;
                helpText += string.Format("{0,2} | {1,2} \n", string.Join(" ", attribute.Texts as List<string>), attribute.HelpText);
            }
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(helpText);

            string infoText = "SystemInfo:\n";
            infoText += new SystemInfo();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine(infoText);
            Console.ResetColor();
        }

        private static List<PropertyInfo> GetArgumentProperties()
        {
            List<PropertyInfo> output = new List<PropertyInfo>();
            PropertyInfo[] properties = typeof(ArgumentParser).GetProperties(BindingFlags.Static | BindingFlags.NonPublic);
            foreach (var property in properties)
                if (Attribute.IsDefined(property, typeof(InputArgumentAttribute)))
                    output.Add(property);
            return output;
        }
    }
}
