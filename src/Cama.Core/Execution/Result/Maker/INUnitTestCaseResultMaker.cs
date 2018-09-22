using System.Collections.Generic;
using System.Xml;

namespace Cama.Core.Execution.Result.Maker
{
    public interface INUnitTestCaseResultMaker
    {
        IList<TestResult> CreateTestCaseResult(XmlNode startNode);
    }
}
