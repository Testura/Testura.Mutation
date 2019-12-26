using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using Microsoft.VisualStudio.PlatformUI;
using Newtonsoft.Json;
using Prism.Mvvm;
using Testura.Mutation.Application.Models;
using Testura.Mutation.Core;
using Testura.Mutation.Core.Solution;
using Testura.Mutation.VsExtension.Services;
using Testura.Mutation.Wpf.Shared.Extensions;
using Testura.Mutation.Wpf.Shared.Models;

namespace Testura.Mutation.VsExtension.Sections.Config
{
    public class MutationConfigWindowViewModel : BindableBase, INotifyPropertyChanged
    {
        private readonly EnvironmentService _environmentService;
        private readonly SolutionInfoService _solutionInfoService;
        private List<string> _projectNamesInSolution;
        private string _solutionPath;

        public MutationConfigWindowViewModel(EnvironmentService environmentService, SolutionInfoService solutionInfoService)
        {
            _environmentService = environmentService;
            _solutionInfoService = solutionInfoService;

            TestRunnerTypes = new List<string> { "DotNet", "xUnit", "NUnit" };
            UpdateConfigCommand = new DelegateCommand(UpdateConfig);
            ProjectGridItems = new ObservableCollection<ConfigProjectGridItem>();
            MutationOperatorGridItems = new ObservableCollection<MutationOperatorGridItem>(Enum
                .GetValues(typeof(MutationOperators)).Cast<MutationOperators>().Select(m =>
                    new MutationOperatorGridItem
                    {
                        IsSelected = true,
                        MutationOperator = m,
                        Description = m.GetValue()
                    }));
        }

        public ObservableCollection<ConfigProjectGridItem> ProjectGridItems { get; set; }

        public ObservableCollection<MutationOperatorGridItem> MutationOperatorGridItems { get; set; }

        public DelegateCommand UpdateConfigCommand { get; set; }

        public List<string> TestRunnerTypes { get; set; }

        public int SelectedTestRunnerIndex { get; set; }

        public bool RunBaseline { get; set; }

        public void Initialize()
        {
            _environmentService.JoinableTaskFactory.RunAsync(async () =>
            {
                await _environmentService.JoinableTaskFactory.SwitchToMainThreadAsync();
                _solutionPath = _environmentService.Dte.Solution.FullName;

                var filePath = GetConfigPath();
                MutationFileConfig mutationFileConfig = null;

                if (File.Exists(filePath))
                {
                    mutationFileConfig = JsonConvert.DeserializeObject<MutationFileConfig>(File.ReadAllText(filePath));
                }

                var projects = await _solutionInfoService.GetSolutionInfoAsync(_solutionPath);
                _projectNamesInSolution = projects.Select(p => p.Name).ToList();

                foreach (var projectName in _projectNamesInSolution)
                {
                    ProjectGridItems.Add(new ConfigProjectGridItem
                    {
                        IsIgnored = mutationFileConfig?.IgnoredProjects.Any(u => u == projectName) ?? false,
                        IsTestProject = mutationFileConfig?.TestProjects.Any(u => u == projectName) ?? false,
                        Name = projectName
                    });
                }

                RunBaseline = mutationFileConfig?.CreateBaseline ?? true;
            });
        }

        private void UpdateConfig()
        {
            var config = new MutationFileConfig
            {
                IgnoredProjects = ProjectGridItems.Where(s => s.IsIgnored).Select(s => s.Name).ToList(),
                SolutionPath = _solutionPath,
                TestProjects = ProjectGridItems.Where(s => s.IsTestProject).Select(s => s.Name).ToList(),
                TestRunner = TestRunnerTypes[SelectedTestRunnerIndex],
                CreateBaseline = RunBaseline
            };

            File.WriteAllText(GetConfigPath(), JsonConvert.SerializeObject(config));

            _environmentService.UserNotificationService.ShowInfoBar<MutationConfigWindow>("Config updated. Note that updates won't affect any currently open mutation windows.");
        }

        private ProjectListItem ConvertToProjectListItem(SolutionProjectInfo solutionProjectInfo, IList<string> projects)
        {
            if (projects == null)
            {
                return new ProjectListItem(solutionProjectInfo, false);
            }

            return new ProjectListItem(solutionProjectInfo, projects.Any(p => p == solutionProjectInfo.Name));
        }

        private string GetConfigPath()
        {
            return Path.Combine(Path.GetDirectoryName(_solutionPath), TesturaMutationVsExtensionPackage.BaseConfigName);
        }
    }
}
