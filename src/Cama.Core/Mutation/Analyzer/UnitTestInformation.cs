using System.Collections.Generic;

namespace Cama.Core.Mutation.Analyzer
{
    public class UnitTestInformation
    {
        public UnitTestInformation()
        {
            ReferencedClasses = new List<string>();
        }

        public string TestName { get; set; }

        public List<string> ReferencedClasses { get; set; }
    }
}
