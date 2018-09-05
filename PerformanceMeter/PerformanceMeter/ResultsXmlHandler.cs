using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using System.Linq;

namespace PerformanceMeter
{
    internal class ResultsXmlHandler
    {
        public void WriteResults(FileInfo resultsFile, TestResults testResults)
        {
            XmlDocument xmlDoc = new XmlDocument();
            XmlNode rootNode;
            if (resultsFile.Exists)
            {
                xmlDoc.Load(resultsFile.FullName);
                rootNode = xmlDoc.GetElementsByTagName("Results").Item(0);
            }
            else
            {
                rootNode = xmlDoc.CreateElement("Results");
                xmlDoc.AppendChild(rootNode);
            }

            XmlNode resultNode = xmlDoc.CreateElement($"{ArgumentParser.AutPath.Name}");
            XmlAttribute cores = xmlDoc.CreateAttribute("cores_count");
            cores.Value = testResults.coresCount.ToString();
            resultNode.Attributes.Append(cores);
            resultNode.InnerText = testResults.executionTime.ToString();
            rootNode.AppendChild(resultNode);
            xmlDoc.Save(resultsFile.FullName);
        }

        public List<TestResults> ReadResults(FileInfo resultsFile)
        {
            XmlDocument xmlDoc = new XmlDocument();
            if (!resultsFile.Exists)
                throw new FileNotFoundException("Failed to find results file", resultsFile.FullName);
            xmlDoc.Load(resultsFile.FullName);
            XmlNodeList results = xmlDoc.SelectNodes($"//Results/{ArgumentParser.AutPath.Name}");
            List<TestResults> output = new List<TestResults>();
            for(int i=0; i<results.Count; i++)
            {
                output.Add(new TestResults(TimeSpan.Parse(results[i].InnerText), uint.Parse(results[i].Attributes["cores_count"].Value)));
            }
            return output;
        }
    }
}
