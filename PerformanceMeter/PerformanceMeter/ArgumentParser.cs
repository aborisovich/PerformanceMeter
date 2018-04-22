using log4net;
using PerformanceMeter.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace PerformanceMeter
{
    /// <summary>
    /// This class parses input arguments of <see cref="global::PerformanceMeter"/> program.
    /// </summary>
    internal static class ArgumentParser
    {
        private static string autPath;
        private static uint processorCount;
        private static List<uint> processorAffinity;
        private static ILog log = LogManager.GetLogger(typeof(ArgumentParser));

        /// <summary>
        /// Path to the Application Under Test.
        /// </summary>
        [InputArgument("Path to the Application Under Test.", "-p")]
        internal static string AutPath
        {
            get { return autPath; }
            private set
            {
                autPath = value;
            }
        }

        /// <summary>
        /// Total count of the physical processor cores that may be used by Application Under Test.
        /// </summary>
        [InputArgument("Number of physical processor cores that may be used by Application Under Test.", "-c")]
        internal static uint ProcessorCount
        {
            get { return processorCount; }
            private set
            {
                processorCount = value;
            }
        }

        /// <summary>
        /// Index numbers of the physical processor cores that can be used by Application Under Test.
        /// </summary>
        /// <exception cref="ArgumentNullException">Thrown on attempt to assign null.</exception>
        /// <exception cref="ArgumentException">Thrown on attempt to assign empty list.</exception>
        [InputArgument("Physical processor alignment.\nAllows to assign specific cores to the Application Under Test.", "-a")]
        internal static List<uint> ProcessorAffinity
        {
            get { return processorAffinity; }
            private set
            {
                if (value.Count == 0)
                    throw new ArgumentException("ProcessorAffinity empty list");
                processorAffinity = value ?? throw new ArgumentNullException("ProcessorAffinity");
            }   
        }

        /// <summary>
        /// Parses input arguments and sets all <see cref="ArgumentParser"/> property values.
        /// </summary>
        /// <param name="args"><see cref="global::PerformanceMeter"/> input arguments.</param>
        public static void ParseArguments(ref string[] args)
        {
            log.InfoFormat("Input arguments:");
            Type myType = typeof(ArgumentParser);
            PropertyInfo[] properties = myType.GetProperties(BindingFlags.Static | BindingFlags.NonPublic);
            foreach(var property in properties)
                ProcessArgument(ref args, myType.GetProperty(property.Name, BindingFlags.Static | BindingFlags.NonPublic));
        }

        /// <summary>
        /// Parses input arguments and sets matching properties with <see cref="InputArgumentAttribute"/>, then removes argument and flag from argument array.
        /// </summary>
        /// <param name="args"><see cref="global::PerformanceMeter"/> input arguments.</param>
        /// <param name="property">A property of <see cref="ArgumentParser"/> having <see cref="InputArgumentAttribute"/>.</param>
        /// <exception cref="ArgumentNullException">Thrown when property argument is null or does not has <see cref="InputArgumentAttribute"/>.</exception>
        /// <exception cref="ArgumentException">Thrown when <see cref="global::PerformanceMeter"/> input arguments are invalid or missing.</exception>
        public static void ProcessArgument(ref string[] args, PropertyInfo property)
        {
            if(!Attribute.IsDefined(property, typeof(InputArgumentAttribute)))
                return;
            InputArgumentAttribute propertyAttribute = property.GetCustomAttribute(typeof(InputArgumentAttribute)) as InputArgumentAttribute;
            int i = 0;
            while (i < args.Length)
            {
                if ((propertyAttribute.Texts as List<string>).Contains(args[i]))
                {
                    if (args.ElementAtOrDefault(i + 1) == null || string.IsNullOrWhiteSpace(args[i + 1]) || i + 1 > args.Length)
                        throw new ArgumentException(String.Format("value of argument: {0} is null, empty string or whitespace", args[i]));
                    else if (property.PropertyType == typeof(string))
                        property.SetValue(null, args[i + 1]);
                    else if (property.PropertyType == typeof(List<uint>))
                    {
                        List<uint> output = new List<uint>();
                        foreach (var value in args[i + 1].Split(","))
                            output.Add(uint.Parse(value));
                        property.SetValue(null, output);
                    } 
                    else if (property.PropertyType == typeof(uint))
                        property.SetValue(null, uint.Parse(args[i + 1]));
                    var temp = args.ToList();
                    temp.RemoveRange(i, 2);
                    args = temp.ToArray();
                    i = 0;
                }
                else
                {
                    i++;
                    continue;
                }
            }
        }
    }
}
