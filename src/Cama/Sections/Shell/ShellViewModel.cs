using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Cama.Core.Execution.Report.Cama;
using Cama.Service.Commands;
using Cama.Service.Commands.Project.OpenProject;
using Cama.Service.Exceptions;
using Cama.Services;
using Cama.Tabs;
using FluentValidation;
using Newtonsoft.Json;
using Prism.Mvvm;

namespace Cama.Sections.Shell
{
    public class ShellViewModel : BindableBase
    {
        private readonly IMutationModuleTabOpener _moduleTabOpener;
        private readonly ILoadingDisplayer _loadingDisplayer;
        private readonly ICommandDispatcher _commandDispatcher;
        private readonly IMutationModuleTabOpener _mutationModuleTabOpener;

        public ShellViewModel(
            IMutationModuleTabOpener moduleTabOpener,
            ILoadingDisplayer loadingDisplayer,
            ICommandDispatcher commandDispatcher,
            IMutationModuleTabOpener mutationModuleTabOpener)
        {
            _moduleTabOpener = moduleTabOpener;
            _loadingDisplayer = loadingDisplayer;
            _commandDispatcher = commandDispatcher;
            _mutationModuleTabOpener = mutationModuleTabOpener;
            MyInterTabClient = new MyInterTabClient();
        }

        public MyInterTabClient MyInterTabClient { get; set; }

        public void Open(IEnumerable<string> paths)
        {
            foreach (var path in paths)
            {
                // Change this to a constant somewhere?
                if (path.EndsWith(".cama", StringComparison.InvariantCultureIgnoreCase))
                {
                    OpenReportAsync(path);
                    continue;
                }

                OpenProjectAsync(path);
            }
        }

        public async void OpenProjectAsync(string path)
        {
            _loadingDisplayer.ShowLoading($"Opening project at {Path.GetFileName(path)}");
            try
            {
                var config = await _commandDispatcher.ExecuteCommandAsync(new OpenProjectCommand(path));
                _mutationModuleTabOpener.OpenOverviewTab(config);
            }
            catch (ValidationException ex)
            {
                ErrorDialogService.ShowErrorDialog("Unexpected error", "Failed to open project.", ex.Message);
            }
            catch (OpenProjectException ex)
            {
                ErrorDialogService.ShowErrorDialog("Unexpected error", "Failed to open project.", ex.InnerException?.ToString());
            }
            finally
            {
                _loadingDisplayer.HideLoading();
            }
        }

        public async void OpenReportAsync(string path)
        {
            try
            {
                _loadingDisplayer.ShowLoading($"Loading report at \"{path}\"");
                var report = await Task.Run(() => JsonConvert.DeserializeObject<CamaReport>(File.ReadAllText(path)));

                if (report == null)
                {
                    ErrorDialogService.ShowErrorDialog(
                        "Unexpected error when loading report",
                        "Report is null (did you try to open an empty file?). Please try to open a different file.");
                    return;
                }

                _moduleTabOpener.OpenTestRunTab(report);
            }
            catch (Exception ex)
            {
                ErrorDialogService.ShowErrorDialog(
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
