using System.Threading;
using NUnit.Engine;

namespace Cama.TestRunner.NUnit
{
    public class NUnitTestRunFinishedTestEventListener : ITestEventListener
    {
        private readonly ManualResetEvent _manualResetEvent;

        public NUnitTestRunFinishedTestEventListener(ManualResetEvent manualResetEvent)
        {
            _manualResetEvent = manualResetEvent;
        }

        public bool TestRunFinished { get; private set; }

        public void OnTestEvent(string report)
        {
            if (report.Contains("test-run"))
            {
                TestRunFinished = true;
                _manualResetEvent.Set();
            }
        }
    }
}
