using log4net;
using PerformanceMeter.Attributes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Linq;

namespace PerformanceMeter
{
    /// <summary>
    /// This class parses input arguments of <see cref="PerformanceMeter"/> program.
    /// </summary>
    internal static class ArgumentParser
    {
        private static string autPath;
        private static List<uint> processorAffinity;
        private static FileInfo outputFile;
        private static ILog log = LogManager.GetLogger(typeof(ArgumentParser));

        /// <summary>
        /// Path to the Application Under Test.
        /// </summary>
        [InputArgument("Path to the Application Under Test.", "-a")]
        internal static string AutPath
        {
            get { return autPath; }
            private set
            {
                autPath = value;
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
                    throw new ArgumentException("ProcessorAffinity argument empty");
                processorAffinity = new List<uint>(value.Split(",", StringSplitOptions.RemoveEmptyEntries).Select(s => uint.Parse(s)));
            }   
        }

        /// <summary>
        /// Path to the output file containing AUT performance test report and analisys.
        /// </summary>
        [InputArgument("Path to the file containing report regarding AUT execution.", "-o")]
        internal static string OutputFile
        {
            get { return outputFile.FullName; }
            private set
            {
                outputFile = new FileInfo(value);
            }
        }

        /// <summary>
        /// Parses input arguments and sets all <see cref="ArgumentParser"/> property values.
        /// </summary>
        /// <param name="args"><see cref="global::PerformanceMeter"/> input arguments.</param>
        public static void ParseArguments(ref string[] args)
        {
            if (args == null || args.Length == 0)
            {
                DisplayHelp(GetArgumentProperties());
                return;
            }
            log.InfoFormat($"Input arguments: {string.Join(" ", args)}");
            try
            {
                SetArgumentProperties(ref args, GetArgumentProperties());
            }
            catch(IndexOutOfRangeException)
            {
                log.Error("Some input arguments does not have a value provided!");
                throw;
            }
            catch(Exception exc)
            {
                log.Error($"Unhandled error occurred: {exc.Message}");
                throw;
            }
        }

        private static void SetArgumentProperties(ref string[] args, List<PropertyInfo> properties)
        {
            foreach (var property in properties)
            {
                InputArgumentAttribute propertyAttribute = property.GetCustomAttribute(typeof(InputArgumentAttribute)) as InputArgumentAttribute;
                    for (int i = 0; i < args.Length; i++)
                    {
                        if ((propertyAttribute.Texts as List<string>).Contains(args[i]))
                            property.SetValue(null, args[i + 1]);
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
