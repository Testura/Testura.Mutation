using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cama.Core.Solution;
using Cama.Infrastructure;
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

        public NewProjectViewModel(FilePickerService filePickerService, SolutionService solutionService, ILoadingDisplayer loadingDisplayer)
        {
            _filePickerService = filePickerService;
            _solutionService = solutionService;
            _loadingDisplayer = loadingDisplayer;
            ProjectNames = new List<string>();
            ProjectPathCommand = new DelegateCommand(ProjectPath);
            SelectedProjects = new List<ProjectListItem>();
        }

        public DelegateCommand ProjectPathCommand { get; set; }

        public string SolutionPath { get; set; }

        public string SelectedTestProject { get; set; }

        public List<string> ProjectNames { get; set; }

        public List<ProjectListItem> SelectedProjects { get; set; }

        private async void ProjectPath()
        {
            var file = _filePickerService.PickFile();
            if (file != null)
            {
                _loadingDisplayer.ShowLoading("Grabbing solution info..");
                SolutionPath = file;
                var projects = await _solutionService.GetSolutionInfoAsync(SolutionPath);
                _loadingDisplayer.HideLoading();
                ProjectNames = projects.Select(p => p.Name).ToList();
                SelectedTestProject = ProjectNames.First();
                SelectedProjects = projects.Select(p => new ProjectListItem(p, false)).ToList();
            }
        }
    }
}
