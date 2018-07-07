using System.Collections.Generic;
using System.Xml;
using NUnit;

namespace Testura.Module.TestRunner.Result.Maker
{
    public class NUnitTestCaseResultMaker : INUnitTestCaseResultMaker
    {
        public IList<NUnitTestCaseResult> CreateTestCaseResult(XmlNode startNode)
        {
            return CreateTestCaseResult(startNode, new List<NUnitTestCaseResult>());
        }

        private IList<NUnitTestCaseResult> CreateTestCaseResult(XmlNode node, IList<NUnitTestCaseResult> results)
        {
            if (node.Name != "test-case")
            {
                foreach (XmlNode childResult in node.ChildNodes)
                    results = CreateTestCaseResult(childResult, results);
            }
            else
            {
                results.Add(new NUnitTestCaseResult
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
