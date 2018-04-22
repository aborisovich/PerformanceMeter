using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace PerformanceMeter.Attributes
{
    /// <summary>
    /// This attribute decorates <see cref="global::PerformanceMeter"/> input arguments in <see cref="ArgumentParser"/>
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    class InputArgumentAttribute : Attribute
    {
        /// <summary>
        /// Instantiates the <see cref="InputArgumentAttribute"/>.
        /// </summary>
        /// <param name="helpText">Description regarding the input argument.</param>
        /// <param name="texts">All flags used to provide input for the specified input argument.</param>
        public InputArgumentAttribute(string helpText, string texts)
        {
            HelpText = helpText;
            Texts = new List<string>() { texts };
        }

        /// <summary>
        /// Instantiates the <see cref="InputArgumentAttribute"/>.
        /// </summary>
        /// <param name="helpText">Description regarding the input argument.</param>
        /// <param name="texts">All flags used to provide input for the specified input argument.</param>
        public InputArgumentAttribute(string helpText, IEnumerable texts)
        {
            HelpText = helpText;
            Texts = texts;
        }

        /// <summary>
        /// Contains description regarding the input argument.
        /// </summary>
        public string HelpText { get; set; }

        /// <summary>
        /// Contains all flags used to provide input for the specified input argument.
        /// </summary>
        public IEnumerable Texts { get; set; }
    }
}
