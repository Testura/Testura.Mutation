using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Threading.Tasks;
using Cama.Core.Execution.Report.Cama;
using Cama.Service.Commands;
using Cama.Service.Commands.Project.History.GetProjectHistory;
using Cama.Service.Commands.Project.OpenProject;
using Cama.Service.Exceptions;
using Cama.Services;
using Cama.Tabs;
using FluentValidation;
using Newtonsoft.Json;
using Prism.Commands;
using Prism.Mvvm;

namespace Cama.Sections.Welcome
{
    public class WelcomeViewModel : BindableBase, INotifyPropertyChanged
    {
        private readonly IMutationModuleTabOpener _mutationModuleTabOpener;
        private readonly IStartModuleTabOpener _startModuleTabOpener;
        private readonly ICommandDispatcher _commandDispatcher;
        private readonly ILoadingDisplayer _loadingDisplayer;
        private readonly FilePickerService _filePickerService;

        public WelcomeViewModel(
            IMutationModuleTabOpener mutationModuleTabOpener,
            IStartModuleTabOpener startModuleTabOpener,
            ICommandDispatcher commandDispatcher,
            ILoadingDisplayer loadingDisplayer,
            FilePickerService filePickerService)
        {
            _mutationModuleTabOpener = mutationModuleTabOpener;
            _startModuleTabOpener = startModuleTabOpener;
            _commandDispatcher = commandDispatcher;
            _loadingDisplayer = loadingDisplayer;
            _filePickerService = filePickerService;
            CreateNewProjectCommand = new DelegateCommand(() => _startModuleTabOpener.OpenNewProjectTab());
            OpenProjectCommand = new DelegateCommand(OpenProject);
            OpenReportCommand = new DelegateCommand(OpenReportAsync);
            OpenHistoryProjectCommand = new DelegateCommand<string>(OpenProjectAsync);
            ProjectHistory = _commandDispatcher.ExecuteCommandAsync(new GetProjectHistoryCommand()).Result;
        }

        public IList<string> ProjectHistory { get; set; }

        public DelegateCommand OpenReportCommand { get; set; }

        public DelegateCommand CreateNewProjectCommand { get; set; }

        public DelegateCommand OpenProjectCommand { get; set; }

        public DelegateCommand<string> OpenHistoryProjectCommand { get; set; }

        private void OpenProject()
        {
            var file = _filePickerService.PickFile(FilePickerService.Filter.Project);
            if (!string.IsNullOrEmpty(file))
            {
                OpenProjectAsync(file);
            }
        }

        private async void OpenReportAsync()
        {
            var file = _filePickerService.PickFile(FilePickerService.Filter.Report);
            if (!string.IsNullOrEmpty(file))
            {
                try
                {
                    _loadingDisplayer.ShowLoading($"Loading report at \"{file}\"");
                    var report = await Task.Run(() => JsonConvert.DeserializeObject<CamaReport>(File.ReadAllText(file)));

                    if (report == null)
                    {
                        ErrorDialogService.ShowErrorDialog(
                            "Unexpected error when loading report",
                            "Report is null (did you try to open an empty file?). Please try to open a different file.");
                        return;
                    }

                    _mutationModuleTabOpener.OpenTestRunTab(report);
                }
                catch (Exception ex)
                {
                    ErrorDialogService.ShowErrorDialog(
                        "Unexpected error when loading report",
                        $"Could not load project at {file}. Please check details for more information.",
                        ex.ToString());
                }
                finally
                {
                    _loadingDisplayer.HideLoading();
                }
            }
        }

        private async void OpenProjectAsync(string path)
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
    }
}
