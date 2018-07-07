using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Testura.Module.TestRunner.Result.Maker
{
    public interface INUnitTestCaseResultMaker
    {
        IList<NUnitTestCaseResult> CreateTestCaseResult(XmlNode startNode);
    }
}
