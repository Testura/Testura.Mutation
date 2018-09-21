using System.Collections.Generic;
using System.IO;
using Cama.Core.Report.Cama;
using Cama.Infrastructure.Tabs;
using Newtonsoft.Json;
using Prism.Mvvm;

namespace Cama.Sections.Shell
{
    public class ShellViewModel : BindableBase
    {
        private readonly IMutationModuleTabOpener _moduleTabOpener;

        public ShellViewModel(IMutationModuleTabOpener moduleTabOpener)
        {
            _moduleTabOpener = moduleTabOpener;
            MyInterTabClient = new MyInterTabClient();
        }

        public MyInterTabClient MyInterTabClient { get; set; }

        public void OpenReport(IEnumerable<string> files)
        {
            foreach (var file in files)
            {
                var report = JsonConvert.DeserializeObject<CamaReport>(File.ReadAllText(file));
                _moduleTabOpener.OpenTestRunTab(report);
            }
        }
    }
}
