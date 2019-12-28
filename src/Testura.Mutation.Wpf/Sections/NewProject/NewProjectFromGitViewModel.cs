using System;
using System.ComponentModel;
using System.Threading.Tasks;
using Prism.Commands;
using Prism.Mvvm;
using Testura.Mutation.Application.Models;
using Testura.Mutation.Core.Git;
using Testura.Mutation.Helpers;
using Testura.Mutation.Helpers.Displayers;
using Testura.Mutation.Infrastructure.Solution;
using Testura.Mutation.Wpf.Helpers.Openers.Tabs;

namespace Testura.Mutation.Sections.NewProject
{
    public class NewProjectFromGitViewModel : BindableBase, INotifyPropertyChanged
    {
        private readonly FilePicker _filePickerService;
        private readonly ILoadingDisplayer _loadingDisplayer;
        private readonly IStartModuleTabOpener _startModuleTabOpener;
        private readonly IGitCloner _gitCloner;
        private readonly SolutionFinder _solutionFinder;

        public NewProjectFromGitViewModel(
            FilePicker filePickerService,
            ILoadingDisplayer loadingDisplayer,
            IStartModuleTabOpener startModuleTabOpener,
            IGitCloner gitCloner,
            SolutionFinder solutionFinder)
        {
            _filePickerService = filePickerService;
            _loadingDisplayer = loadingDisplayer;
            _startModuleTabOpener = startModuleTabOpener;
            _gitCloner = gitCloner;
            _solutionFinder = solutionFinder;
            ProjectPathCommand = new DelegateCommand(PickProjectPath);
            GoNextCommand = new DelegateCommand(async () => await GoNext());
            BranchName = "master";
        }

        public string LocalPath { get; set; }

        public string RepositoryUrl { get; set; }

        public string BranchName { get; set; }

        public string Username { get; set; }

        public string Password { get; set; }

        public DelegateCommand ProjectPathCommand { get; set; }

        public DelegateCommand GoNextCommand { get; set; }

        private async Task GoNext()
        {
            var result = CommonDialogDisplayer.ShowInfoDialog("We will now clone and build the project. Are you sure? ");

            if (!result)
            {
                return;
            }

            try
            {
                _loadingDisplayer.ShowLoading("Cloning project and building project..");
                await _gitCloner.CloneSolutionAsync(RepositoryUrl, BranchName, Username, Password, LocalPath);

                _loadingDisplayer.ShowLoading("Looking for solution..");
                var solutionPath = _solutionFinder.FindSolution(LocalPath);

                if (solutionPath == null)
                {
                    throw new Exception($"Could not find any solution file in the {LocalPath} directory or subdirectories.");
                }

                var gitInfo = new GitInfo
                {
                    RepositoryUrl = RepositoryUrl,
                    Branch = BranchName,
                    LocalPath = LocalPath,
                    Username = Username,
                    Password = Password
                };

                _loadingDisplayer.HideLoading();

                _startModuleTabOpener.OpenNewProjectTab(gitInfo, solutionPath);
            }
            catch (Exception ex)
            {
                _loadingDisplayer.HideLoading();
                CommonDialogDisplayer.ShowErrorDialog("Error when cloning project", ex.Message, ex.ToString());
            }
        }

        private void PickProjectPath()
        {
            var directory = _filePickerService.PickDirectory();
            if (!string.IsNullOrEmpty(directory))
            {
                LocalPath = directory;
            }
        }
    }
}
