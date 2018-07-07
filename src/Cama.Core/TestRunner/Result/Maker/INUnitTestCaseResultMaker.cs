using System.Collections.Generic;
using System.Xml;

namespace Cama.Core.TestRunner.Result.Maker
{
    public interface INUnitTestCaseResultMaker
    {
        IList<NUnitTestCaseResult> CreateTestCaseResult(XmlNode startNode);
    }
}
