using System.Collections.Generic;
using System.ComponentModel;
using MediatR;
using Prism.Commands;
using Prism.Mvvm;
using Unima.Application.Commands.Project.History.GetProjectHistory;
using Unima.Helpers;
using Unima.Helpers.Openers;
using Unima.Helpers.Openers.Tabs;

namespace Unima.Sections.Welcome
{
    public class WelcomeViewModel : BindableBase, INotifyPropertyChanged
    {
        private readonly IStartModuleTabOpener _startModuleTabOpener;
        private readonly IMediator _mediator;
        private readonly MutationReportOpener _mutationReportOpener;
        private readonly UnimaProjectOpener _projectOpener;
        private readonly FilePicker _filePickerService;

        public WelcomeViewModel(
            IStartModuleTabOpener startModuleTabOpener,
            IMediator mediator,
            MutationReportOpener mutationReportOpener,
            UnimaProjectOpener projectOpener,
            FilePicker filePickerService)
        {
            _startModuleTabOpener = startModuleTabOpener;
            _mediator = mediator;
            _mutationReportOpener = mutationReportOpener;
            _projectOpener = projectOpener;
            _filePickerService = filePickerService;
            CreateNewProjectCommand = new DelegateCommand(() => _startModuleTabOpener.OpenNewProjectTab());
            OpenProjectCommand = new DelegateCommand(OpenProject);
            OpenReportCommand = new DelegateCommand(OpenReport);
            OpenHistoryProjectCommand = new DelegateCommand<string>(OpenProject);
            ProjectHistory = _mediator.Send(new GetProjectHistoryCommand()).Result;
        }

        public IList<string> ProjectHistory { get; set; }

        public DelegateCommand OpenReportCommand { get; set; }

        public DelegateCommand CreateNewProjectCommand { get; set; }

        public DelegateCommand OpenProjectCommand { get; set; }

        public DelegateCommand<string> OpenHistoryProjectCommand { get; set; }

        private void OpenProject()
        {
            var file = _filePickerService.PickFile(FilePicker.Filter.Project);
            if (!string.IsNullOrEmpty(file))
            {
                OpenProject(file);
            }
        }

        private void OpenReport()
        {
            var file = _filePickerService.PickFile(FilePicker.Filter.Report);
            if (!string.IsNullOrEmpty(file))
            {
                _mutationReportOpener.OpenMutationReport(file);
            }
        }

        private void OpenProject(string path)
        {
            _projectOpener.OpenProject(path);
        }
    }
}
