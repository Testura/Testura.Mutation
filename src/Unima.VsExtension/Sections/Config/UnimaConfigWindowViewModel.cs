using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using EnvDTE;
using Microsoft.VisualStudio.PlatformUI;
using Microsoft.VisualStudio.Threading;
using Newtonsoft.Json;
using Prism.Mvvm;
using Unima.Application.Models;
using Unima.Core.Solution;
using Unima.Wpf.Shared.Models;

namespace Unima.VsExtension.Sections.Config
{
    public class UnimaConfigWindowViewModel : BindableBase, INotifyPropertyChanged
    {
        private readonly SolutionInfoService _solutionInfoService;
        private string _solutionPath;

        public UnimaConfigWindowViewModel(SolutionInfoService solutionInfoService)
        {
            _solutionInfoService = solutionInfoService;
            TestRunnerTypes = new List<string> { "DotNet", "xUnit", "NUnit" };
            ProjectNamesInSolution = new List<string>();
            IgnoredProjectsInSolution = new List<ProjectListItem>();
            UpdateConfigCommand = new DelegateCommand(UpdateConfig);
        }

        public DelegateCommand UpdateConfigCommand { get; set; }

        public List<ProjectListItem> SelectedTestProjectInSolution { get; set; }

        public List<string> ProjectNamesInSolution { get; set; }

        public List<ProjectListItem> IgnoredProjectsInSolution { get; set; }

        public List<string> TestRunnerTypes { get; set; }

        public int SelectedTestRunnerIndex { get; set; }

        public bool RunBaseline { get; set; }

        public void Initialize(DTE dte, JoinableTaskFactory joinableTaskFactory)
        {
            joinableTaskFactory.RunAsync(async () =>
            {
                await joinableTaskFactory.SwitchToMainThreadAsync();
                _solutionPath = dte.Solution.FullName;

                var filePath = GetConfigPath();
                UnimaFileConfig unimaFileConfig = null;

                if (File.Exists(filePath))
                {
                    unimaFileConfig = JsonConvert.DeserializeObject<UnimaFileConfig>(File.ReadAllText(filePath));
                }

                var projects = await _solutionInfoService.GetSolutionInfoAsync(_solutionPath);
                ProjectNamesInSolution = projects.Select(p => p.Name).ToList();
                SelectedTestProjectInSolution = projects.Select(p => ConvertToProjectListItem(p, unimaFileConfig?.TestProjects)).ToList();
                IgnoredProjectsInSolution = projects.Select(p => ConvertToProjectListItem(p, unimaFileConfig?.IgnoredProjects)).ToList();
                RunBaseline = unimaFileConfig != null ? unimaFileConfig.CreateBaseline : true;
            });
        }

        private void UpdateConfig()
        {
            var config = new UnimaFileConfig
            {
                IgnoredProjects = IgnoredProjectsInSolution.Where(s => s.IsSelected).Select(s => s.ProjectInfo.Name).ToList(),
                SolutionPath = _solutionPath,
                TestProjects = SelectedTestProjectInSolution.Where(s => s.IsSelected).Select(s => s.ProjectInfo.Name).ToList(),
                TestRunner = TestRunnerTypes[SelectedTestRunnerIndex],
                CreateBaseline = RunBaseline
            };

            File.WriteAllText(GetConfigPath(), JsonConvert.SerializeObject(config));
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
            return Path.Combine(Path.GetDirectoryName(_solutionPath), UnimaVsExtensionPackage.BaseConfigName);
        }
    }
}
