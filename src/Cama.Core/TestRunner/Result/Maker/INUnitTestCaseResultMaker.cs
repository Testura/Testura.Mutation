using System.Collections.Generic;
using System.Xml;
using TestResult = Cama.Core.Models.TestResult;

namespace Cama.Core.TestRunner.Result.Maker
{
    public interface INUnitTestCaseResultMaker
    {
        IList<TestResult> CreateTestCaseResult(XmlNode startNode);
    }
}
