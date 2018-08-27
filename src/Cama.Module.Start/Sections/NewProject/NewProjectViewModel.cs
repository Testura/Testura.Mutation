using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cama.Core.Models.Mutation;
using Cama.Core.Models.Project;
using Cama.Core.Services.Project;
using Cama.Core.Solution;
using Cama.Infrastructure;
using Cama.Infrastructure.Services;
using Cama.Infrastructure.Tabs;
using Cama.Module.Start.Models;
using Cama.Module.Start.Services;
using Prism.Commands;
using Prism.Mvvm;

namespace Cama.Module.Start.Sections.NewProject
{
    public class NewProjectViewModel : BindableBase, INotifyPropertyChanged
    {
        private readonly FilePickerService _filePickerService;
        private readonly SolutionService _solutionService;
        private readonly ILoadingDisplayer _loadingDisplayer;
        private readonly ICreateProjectService _createProjectService;
        private readonly IMutationModuleTabOpener _mutationModuleTabOpener;
        private readonly ProjectHistoryService _projectHistoryService;

        public NewProjectViewModel(
            FilePickerService filePickerService,
            SolutionService solutionService,
            ILoadingDisplayer loadingDisplayer,
            ICreateProjectService createProjectService,
            IMutationModuleTabOpener mutationModuleTabOpener,
            ProjectHistoryService projectHistoryService)
        {
            _filePickerService = filePickerService;
            _solutionService = solutionService;
            _loadingDisplayer = loadingDisplayer;
            _createProjectService = createProjectService;
            _mutationModuleTabOpener = mutationModuleTabOpener;
            _projectHistoryService = projectHistoryService;
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
            var file = _filePickerService.PickFile();
            if (!string.IsNullOrEmpty(file))
            {
                _loadingDisplayer.ShowLoading("Grabbing solution info..");
                SolutionPath = file;
                var projects = await _solutionService.GetSolutionInfoAsync(SolutionPath);
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


        private void CreateProject()
        {
            var projectPath = Path.Combine(ProjectPath, $"{ProjectName}.json");


            var config = new CamaLocalConfig
            {
                MutationProjects = SelectedProjectsInSolution.Where(s => s.IsSelected).Select(s => s.ProjectInfo.Name).ToList(),
                SolutionPath = SolutionPath,
                TestProjects = SelectedTestProjectInSolution.Where(s => s.IsSelected).Select(s => s.ProjectInfo.Name).ToList(),
            };


            _createProjectService.CreateProject(projectPath, config);
            _projectHistoryService.AddToHistory(projectPath);
            _mutationModuleTabOpener.OpenOverviewTab(null);
        }
    }
}
