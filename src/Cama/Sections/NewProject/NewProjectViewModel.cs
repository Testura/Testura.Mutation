using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using Cama.Core.Solution;
using Cama.Models;
using Cama.Service.Commands;
using Cama.Service.Commands.Project.CreateProject;
using Cama.Service.Commands.Project.History.AddProjectHistory;
using Cama.Service.Commands.Project.OpenProject;
using Cama.Service.Models;
using Cama.Services;
using Cama.Tabs;
using Prism.Commands;
using Prism.Mvvm;

namespace Cama.Sections.NewProject
{
    public class NewProjectViewModel : BindableBase, INotifyPropertyChanged
    {
        private readonly FilePickerService _filePickerService;
        private readonly SolutionInfoService _solutionInfoService;
        private readonly ILoadingDisplayer _loadingDisplayer;
        private readonly ICommandDispatcher _commandDispatcher;
        private readonly IMutationModuleTabOpener _mutationModuleTabOpener;

        public NewProjectViewModel(
            FilePickerService filePickerService,
            SolutionInfoService solutionInfoService,
            ILoadingDisplayer loadingDisplayer,
            ICommandDispatcher commandDispatcher,
            IMutationModuleTabOpener mutationModuleTabOpener)
        {
            _filePickerService = filePickerService;
            _solutionInfoService = solutionInfoService;
            _loadingDisplayer = loadingDisplayer;
            _commandDispatcher = commandDispatcher;
            _mutationModuleTabOpener = mutationModuleTabOpener;
            ProjectNamesInSolution = new List<string>();
            ProjectPathCommand = new DelegateCommand(PickProjectPath);
            SolutionPathCommand = new DelegateCommand(PickSolutionPathAsync);
            CreateProjectCommand = new DelegateCommand(CreateProject);
            SelectedProjectsInSolution = new List<ProjectListItem>();
        }

        public DelegateCommand SolutionPathCommand { get; set; }

        public DelegateCommand ProjectPathCommand { get; set; }

        public DelegateCommand CreateProjectCommand { get; set; }

        public string ProjectName { get; set; }

        public string ProjectPath { get; set; }

        public string SolutionPath { get; set; }

        public List<ProjectListItem> SelectedTestProjectInSolution { get; set; }

        public List<string> ProjectNamesInSolution { get; set; }

        public List<ProjectListItem> SelectedProjectsInSolution { get; set; }

        private async void PickSolutionPathAsync()
        {
            var file = _filePickerService.PickFile(FilePickerService.Filter.Solution);
            if (!string.IsNullOrEmpty(file))
            {
                _loadingDisplayer.ShowLoading("Grabbing solution info..");
                SolutionPath = file;
                var projects = await _solutionInfoService.GetSolutionInfoAsync(SolutionPath);
                _loadingDisplayer.HideLoading();
                ProjectNamesInSolution = projects.Select(p => p.Name).ToList();
                SelectedTestProjectInSolution = projects.Select(p => new ProjectListItem(p, false)).ToList();
                SelectedProjectsInSolution = projects.Select(p => new ProjectListItem(p, false)).ToList();
            }
        }

        private void PickProjectPath()
        {
            var directory = _filePickerService.PickDirectory();
            if (!string.IsNullOrEmpty(directory))
            {
                ProjectPath = directory;
            }
        }

        private async void CreateProject()
        {
            var projectPath = Path.Combine(ProjectPath, $"{ProjectName}.json");

            var config = new CamaFileConfig
            {
                IgnoredMutationProjects = SelectedProjectsInSolution.Where(s => !s.IsSelected).Select(s => s.ProjectInfo.Name).ToList(),
                SolutionPath = SolutionPath,
                TestProjects = SelectedTestProjectInSolution.Where(s => s.IsSelected).Select(s => s.ProjectInfo.Name).ToList(),
            };

            await _commandDispatcher.ExecuteCommandAsync(new CreateProjectCommand(projectPath, config));
            await _commandDispatcher.ExecuteCommandAsync(new AddProjectHistoryCommand(projectPath));
            _mutationModuleTabOpener.OpenOverviewTab(await _commandDispatcher.ExecuteCommandAsync(new OpenProjectCommand(projectPath)));
        }
    }
}
