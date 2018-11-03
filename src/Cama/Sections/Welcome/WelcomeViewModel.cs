using System.Collections.Generic;
using System.ComponentModel;
using Cama.Helpers;
using Cama.Helpers.Openers;
using Cama.Helpers.Openers.Tabs;
using Cama.Service.Commands.Project.History.GetProjectHistory;
using MediatR;
using Prism.Commands;
using Prism.Mvvm;

namespace Cama.Sections.Welcome
{
    public class WelcomeViewModel : BindableBase, INotifyPropertyChanged
    {
        private readonly IStartModuleTabOpener _startModuleTabOpener;
        private readonly IMediator _mediator;
        private readonly MutationReportOpener _mutationReportOpener;
        private readonly CamaProjectOpener _projectOpener;
        private readonly FilePicker _filePickerService;

        public WelcomeViewModel(
            IStartModuleTabOpener startModuleTabOpener,
            IMediator mediator,
            MutationReportOpener mutationReportOpener,
            CamaProjectOpener projectOpener,
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
