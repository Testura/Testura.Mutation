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
using Testura.Mutation.VsExtension.Wrappers;
using Testura.Mutation.Wpf.Shared.Extensions;
using Testura.Mutation.Wpf.Shared.Models;

namespace Testura.Mutation.VsExtension.Sections.Config
{
    public class MutationConfigWindowViewModel : BindableBase, INotifyPropertyChanged
    {
        private readonly EnvironmentWrapper _environmentWrapper;
        private readonly SolutionInfoService _solutionInfoService;
        private List<string> _projectNamesInSolution;
        private string _solutionPath;

        public MutationConfigWindowViewModel(EnvironmentWrapper environmentWrapper, SolutionInfoService solutionInfoService)
        {
            _environmentWrapper = environmentWrapper;
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
            _environmentWrapper.JoinableTaskFactory.RunAsync(async () =>
            {
                await _environmentWrapper.JoinableTaskFactory.SwitchToMainThreadAsync();
                _solutionPath = _environmentWrapper.Dte.Solution.FullName;

                var filePath = GetConfigPath();
                TesturaMutationFileConfig testuraMutationFileConfig = null;

                if (File.Exists(filePath))
                {
                    testuraMutationFileConfig = JsonConvert.DeserializeObject<TesturaMutationFileConfig>(File.ReadAllText(filePath));
                }

                var projects = await _solutionInfoService.GetSolutionInfoAsync(_solutionPath);
                _projectNamesInSolution = projects.Select(p => p.Name).ToList();

                foreach (var projectName in _projectNamesInSolution)
                {
                    ProjectGridItems.Add(new ConfigProjectGridItem
                    {
                        IsIgnored = testuraMutationFileConfig?.IgnoredProjects.Any(u => u == projectName) ?? false,
                        IsTestProject = testuraMutationFileConfig?.TestProjects.Any(u => u == projectName) ?? false,
                        Name = projectName
                    });
                }

                RunBaseline = testuraMutationFileConfig?.CreateBaseline ?? true;
            });
        }

        private void UpdateConfig()
        {
            var config = new TesturaMutationFileConfig
            {
                IgnoredProjects = ProjectGridItems.Where(s => s.IsIgnored).Select(s => s.Name).ToList(),
                SolutionPath = _solutionPath,
                TestProjects = ProjectGridItems.Where(s => s.IsTestProject).Select(s => s.Name).ToList(),
                TestRunner = TestRunnerTypes[SelectedTestRunnerIndex],
                CreateBaseline = RunBaseline
            };

            File.WriteAllText(GetConfigPath(), JsonConvert.SerializeObject(config));

            _environmentWrapper.UserNotificationService.ShowMessage("Config updated. Updates won't affect any currently open mutation windows.");
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
