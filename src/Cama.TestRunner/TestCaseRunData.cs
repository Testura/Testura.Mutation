using System;

namespace Testura.Module.TestRunner
{
    [Serializable]
    public class TestCaseRunData
    {
        public string TestCaseName { get; set; }
        public string[] DataSets { get; set; }

        public TestCaseRunData(string testCaseName, string[] dataSets)
        {
            TestCaseName = testCaseName;
            DataSets = dataSets;
        }
    }
}
