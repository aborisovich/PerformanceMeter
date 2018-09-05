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
        public readonly TestResults testResults;

        public ResultsXmlHandler(TestResults testResults)
        {
            this.testResults = testResults;
        }

        public void WriteResults(FileInfo resultsFile)
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

            XmlNode resultNode = xmlDoc.CreateElement($"{testResults.autName}");
            XmlAttribute cores = xmlDoc.CreateAttribute("cores");
            cores.Value = testResults.coresCount.ToString();
            resultNode.Attributes.Append(cores);
            resultNode.InnerText = testResults.executionTime.ToString();
            rootNode.AppendChild(resultNode);
            xmlDoc.Save(resultsFile.FullName);
        }
    }
}
