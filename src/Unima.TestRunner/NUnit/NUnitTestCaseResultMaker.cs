using System;
using System.Collections.Generic;
using System.Xml;
using NUnit;
using Unima.Core.Execution.Result;

namespace Unima.TestRunner.NUnit
{
    public class NUnitTestCaseResultMaker
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
                var label = node.GetAttribute("label");

                results.Add(new TestResult
                {
                    FullName = node.GetAttribute("fullname"),
                    Name = node.GetAttribute("name"),
                    IsSuccess = node.GetAttribute("result") == "Passed" || (label != null && label.Equals("ignored", StringComparison.InvariantCultureIgnoreCase)),
                    InnerText = string.IsNullOrEmpty(node.InnerText) ? "Test passed without any errors." : node.InnerText
                });
            }

            return results;
        }
    }
}
