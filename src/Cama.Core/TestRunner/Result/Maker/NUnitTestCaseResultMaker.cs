using System.Collections.Generic;
using System.Xml;
using NUnit;

namespace Cama.Core.TestRunner.Result.Maker
{
    public class NUnitTestCaseResultMaker : INUnitTestCaseResultMaker
    {
        public IList<TestResult> CreateTestCaseResult(XmlNode startNode)
        {
            return CreateTestCaseResult(startNode, new List<TestResult>());
        }

        private IList<TestResult> CreateTestCaseResult(XmlNode node, IList<TestResult> results)
        {
            if (node.Name != "test-case")
            {
                foreach (XmlNode childResult in node.ChildNodes)
                {
                    results = CreateTestCaseResult(childResult, results);
                }
            }
            else
            {
                results.Add(new TestResult
                {
                    FullName = node.GetAttribute("fullname"),
                    Name = node.GetAttribute("name"),
                    IsSuccess = node.GetAttribute("result") == "Passed",
                    InnerText = string.IsNullOrEmpty(node.InnerText) ? "Test passed without any errors." : node.InnerText
                });
            }

            return results;
        }
    }
}
