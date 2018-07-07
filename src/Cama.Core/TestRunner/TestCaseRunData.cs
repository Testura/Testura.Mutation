using System;

namespace Cama.Core.TestRunner
{
    [Serializable]
    public class TestCaseRunData
    {
        public TestCaseRunData(string testCaseName, string[] dataSets)
        {
            TestCaseName = testCaseName;
            DataSets = dataSets;
        }

        public string TestCaseName { get; set; }

        public string[] DataSets { get; set; }
    }
}
