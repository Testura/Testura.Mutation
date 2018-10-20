using System;
using System.Collections.Generic;
using System.IO;
using Cama.Core.Execution.Report.Cama;
using Cama.Services;
using Cama.Tabs;
using Newtonsoft.Json;
using Prism.Mvvm;

namespace Cama.Sections.Shell
{
    public class ShellViewModel : BindableBase
    {
        private readonly IMutationModuleTabOpener _moduleTabOpener;
        private readonly ILoadingDisplayer _loadingDisplayer;

        public ShellViewModel(IMutationModuleTabOpener moduleTabOpener, ILoadingDisplayer loadingDisplayer)
        {
            _moduleTabOpener = moduleTabOpener;
            _loadingDisplayer = loadingDisplayer;
            MyInterTabClient = new MyInterTabClient();
        }

        public MyInterTabClient MyInterTabClient { get; set; }

        public void OpenReport(IEnumerable<string> files)
        {
            foreach (var file in files)
            {
                try
                {
                    _loadingDisplayer.ShowLoading($"Loading project at \"{file}\"");
                    var report = JsonConvert.DeserializeObject<CamaReport>(File.ReadAllText(file));
                    _moduleTabOpener.OpenTestRunTab(report);
                }
                catch (Exception ex)
                {
                    ErrorDialogService.ShowErrorDialog(
                        "Unexpected error when loading project",
                        $"Could not load project at {file}. Please check details for more information.",
                        ex.ToString());
                }
                finally
                {
                    _loadingDisplayer.HideLoading();
                }
            }
        }
    }
}
