using System;
using System.IO;
using System.Threading.Tasks;
using Cama.Core.Execution.Report.Cama;
using Cama.Helpers.Displayers;
using Cama.Helpers.Openers.Tabs;
using Newtonsoft.Json;

namespace Cama.Helpers.Openers
{
    public class MutationReportOpener
    {
        private readonly ILoadingDisplayer _loadingDisplayer;
        private readonly IMutationModuleTabOpener _mutationModuleTabOpener;

        public MutationReportOpener(ILoadingDisplayer loadingDisplayer, IMutationModuleTabOpener mutationModuleTabOpener)
        {
            _loadingDisplayer = loadingDisplayer;
            _mutationModuleTabOpener = mutationModuleTabOpener;
        }

        public async void OpenMutationReport(string path)
        {
            try
            {
                _loadingDisplayer.ShowLoading($"Loading report at \"{path}\"");
                var report = await Task.Run(() => JsonConvert.DeserializeObject<CamaReport>(File.ReadAllText(path)));

                if (report?.Mutations == null)
                {
                    ErrorDialogDisplayer.ShowErrorDialog(
                        "Unexpected error when loading report",
                        "Report is null (did you try to open an empty file?). Please try to open a different file.");
                    return;
                }

                _mutationModuleTabOpener.OpenTestRunTab(report);
            }
            catch (Exception ex)
            {
                ErrorDialogDisplayer.ShowErrorDialog(
                    "Unexpected error when loading report",
                    $"Could not load project at {path}. Please check details for more information.",
                    ex.ToString());
            }
            finally
            {
                _loadingDisplayer.HideLoading();
            }
        }
    }
}
