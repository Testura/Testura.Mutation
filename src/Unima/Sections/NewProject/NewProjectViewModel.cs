using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using MediatR;
using Prism.Commands;
using Prism.Mvvm;
using Unima.Application.Commands.Project.CreateProject;
using Unima.Application.Commands.Project.History.AddProjectHistory;
using Unima.Application.Commands.Project.OpenProject;
using Unima.Application.Models;
using Unima.Core.Solution;
using Unima.Helpers;
using Unima.Helpers.Displayers;
using Unima.Helpers.Openers.Tabs;
using Unima.Models;

namespace Unima.Sections.NewProject
{
    public class NewProjectViewModel : BindableBase, INotifyPropertyChanged
    {
        private readonly FilePicker _filePickerService;
        private readonly SolutionInfoService _solutionInfoService;
        private readonly ILoadingDisplayer _loadingDisplayer;
        private readonly IMediator _mediator;
        private readonly IMutationModuleTabOpener _mutationModuleTabOpener;

        public NewProjectViewModel(
            FilePicker filePickerService,
            SolutionInfoService solutionInfoService,
            ILoadingDisplayer loadingDisplayer,
            IMediator mediator,
            IMutationModuleTabOpener mutationModuleTabOpener)
        {
            _filePickerService = filePickerService;
            _solutionInfoService = solutionInfoService;
            _loadingDisplayer = loadingDisplayer;
            _mediator = mediator;
            _mutationModuleTabOpener = mutationModuleTabOpener;
            TestRunnerTypes = new List<string> { "DotNet", "xUnit", "NUnit" };
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

        public List<string> TestRunnerTypes { get; set; }

        public int SelectedTestRunnerIndex { get; set; }

        public void InitializeWithGitInfo(GitInfo gitInfo, string solutionPath)
        {
            SolutionPath = solutionPath;
            LoadSolutionInfo();
        }

        private void PickSolutionPathAsync()
        {
            var file = _filePickerService.PickFile(FilePicker.Filter.Solution);
            if (!string.IsNullOrEmpty(file))
            {
                SolutionPath = file;
                LoadSolutionInfo();
            }
        }

        private async void LoadSolutionInfo()
        {
            _loadingDisplayer.ShowLoading("Grabbing solution info..");
            var projects = await _solutionInfoService.GetSolutionInfoAsync(SolutionPath);
            _loadingDisplayer.HideLoading();
            ProjectNamesInSolution = projects.Select(p => p.Name).ToList();
            SelectedTestProjectInSolution = projects.Select(p => new ProjectListItem(p, false)).ToList();
            SelectedProjectsInSolution = projects.Select(p => new ProjectListItem(p, false)).ToList();
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

            var config = new UnimaFileConfig
            {
                IgnoredProjects = new List<string>(),
                SolutionPath = SolutionPath,
                TestProjects = SelectedTestProjectInSolution.Where(s => s.IsSelected).Select(s => s.ProjectInfo.Name).ToList(),
                TestRunner = TestRunnerTypes[SelectedTestRunnerIndex]
            };

            await _mediator.Send(new CreateProjectCommand(projectPath, config));
            await _mediator.Send(new AddProjectHistoryCommand(projectPath));
            _mutationModuleTabOpener.OpenOverviewTab(await _mediator.Send(new OpenProjectCommand(projectPath)));
        }
    }
}
