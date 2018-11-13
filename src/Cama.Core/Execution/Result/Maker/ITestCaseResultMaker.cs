using System.Collections.Generic;
using System.Xml;

namespace Cama.Core.Execution.Result.Maker
{
    public interface ITestCaseResultMaker
    {
        IList<TestResult> CreateTestCaseResult(XmlNode startNode);
    }
}
