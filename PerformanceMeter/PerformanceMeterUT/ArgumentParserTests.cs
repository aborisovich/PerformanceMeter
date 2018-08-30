using Microsoft.VisualStudio.TestTools.UnitTesting;
using PerformanceMeter;
using System;
using System.IO;
using System.Reflection;

namespace InputOutputTests
{
    [TestClass]
    public class ArgumentParserTests
    {
        private const string autName = "testAut";

        [TestInitialize]
        public void Initialize()
        {
            File.Create(Path.Combine(Directory.GetCurrentDirectory(), autName)).Close();
        }

        [TestCleanup]
        public void CleanUp()
        {
            File.Delete(Path.Combine(Directory.GetCurrentDirectory(), autName));
        }

        [TestMethod, TestCategory("InputArgumentsTests")]
        public void ParsingAutPathWindowsPositive()
        {
            string[] inputArgs = new string[] { "-a", Path.Combine(Directory.GetCurrentDirectory(), autName) };
            ArgumentParser.ParseArguments(ref inputArgs);
            Assert.IsTrue(!string.IsNullOrWhiteSpace(ArgumentParser.AutPath.FullName));
            Assert.AreEqual(inputArgs[1], ArgumentParser.AutPath.FullName);
            Assert.IsTrue(Path.IsPathRooted(ArgumentParser.AutPath.FullName));
        }

        [TestMethod, TestCategory("InputArgumentsTests")]
        public void ParsingAutPathUnixPositive()
        {
            string[] inputArgs = new string[] { "-a", Path.Combine(Directory.GetCurrentDirectory(), autName).Replace(@"\", @"/") };
            ArgumentParser.ParseArguments(ref inputArgs);
            Assert.IsTrue(!string.IsNullOrWhiteSpace(ArgumentParser.AutPath.FullName));
            Assert.AreEqual(inputArgs[1], ArgumentParser.AutPath.FullName.Replace(@"\", @"/"));
            Assert.IsTrue(Path.IsPathRooted(ArgumentParser.AutPath.FullName));
        }

        [TestMethod, TestCategory("InputArgumentsTests")]
        public void ParsingProcessorAffinityPositive()
        {
            string[] inputArgs = new string[] { "-c", "1, 2, 3" };
            ArgumentParser.ParseArguments(ref inputArgs);
            Assert.IsTrue(!string.IsNullOrWhiteSpace(ArgumentParser.ProcessorAffinity));
            string[] inputNumbers = inputArgs[1].Split(",", StringSplitOptions.RemoveEmptyEntries);
            string[] outputNumbers = ArgumentParser.ProcessorAffinity.Split(",", StringSplitOptions.RemoveEmptyEntries);
            Assert.AreEqual(inputNumbers.Length, outputNumbers.Length);
            for (int i = 0; i < inputNumbers.Length; i++)
                Assert.AreEqual(uint.Parse(inputNumbers[i]), uint.Parse(outputNumbers[i]));
        }

        [TestMethod, TestCategory("InputArgumentsTests")]
        [ExpectedException(typeof(ArgumentException), "Value can not be set to 0")]
        public void ParsingProcessorAffinityNegativeZeroValue()
        {
            string[] inputArgs = new string[] { "-c", "0, 2, 1" };
            try
            {
                ArgumentParser.ParseArguments(ref inputArgs);
            }
            catch(TargetInvocationException e)
            {
                throw e.InnerException;
            }
        }

        [TestMethod, TestCategory("InputArgumentsTests")]
        public void ParsingOutputFileWindowsPositive()
        {
            string[] inputArgs = new string[] { "-o", Path.Combine(Directory.GetCurrentDirectory(), "report.txt") };
            ArgumentParser.ParseArguments(ref inputArgs);
            Assert.IsTrue(!string.IsNullOrWhiteSpace(ArgumentParser.OutputFile.FullName));
            Assert.AreEqual(inputArgs[1], ArgumentParser.OutputFile.FullName);
            Assert.IsTrue(Path.IsPathRooted(ArgumentParser.OutputFile.FullName));
        }

        [TestMethod, TestCategory("InputArgumentsTests")]
        public void ParsingOutputFileUnixPositive()
        {
            string[] inputArgs = new string[] { "-o", Path.Combine(Directory.GetCurrentDirectory(), "report.txt").Replace(@"\", @"/")};
            ArgumentParser.ParseArguments(ref inputArgs);
            Assert.IsTrue(!string.IsNullOrWhiteSpace(ArgumentParser.OutputFile.FullName));
            Assert.AreEqual(inputArgs[1], ArgumentParser.OutputFile.FullName.Replace(@"\", @"/"));
            Assert.IsTrue(Path.IsPathRooted(ArgumentParser.OutputFile.FullName));
        }
    }
}
