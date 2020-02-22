using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using Microsoft.Practices.ObjectBuilder2;
using Microsoft.VisualStudio.PlatformUI;
using Newtonsoft.Json;
using Prism.Mvvm;
using Testura.Mutation.Application.Models;
using Testura.Mutation.Core;
using Testura.Mutation.Core.Creator.Filter;
using Testura.Mutation.Core.Solution;
using Testura.Mutation.VsExtension.Services;
using Testura.Mutation.VsExtension.Util.Extensions;
using ConfigProjectGridItem = Testura.Mutation.VsExtension.Models.ConfigProjectGridItem;
using ConfigProjectMappingGridItem = Testura.Mutation.VsExtension.Models.ConfigProjectMappingGridItem;
using MutationOperatorGridItem = Testura.Mutation.VsExtension.Models.MutationOperatorGridItem;

namespace Testura.Mutation.VsExtension.Sections.Config
{
    public class MutationConfigWindowViewModel : BindableBase
    {
        private readonly EnvironmentService _environmentService;
        private readonly SolutionInfoService _solutionInfoService;
        private List<string> _projectNamesInSolution;
        private string _solutionPath;

        private int _selectedTestRunnerIndex;
        private bool runBaseline;
        private int _numberOfParallelTestRuns;
        private MutationFileConfig _mutationFileConfig;
        private string _filter;
        private JsonSerializerSettings _jsonSettings;
        private int _selectedEffectIndex;

        public MutationConfigWindowViewModel(EnvironmentService environmentService, SolutionInfoService solutionInfoService)
        {
            _environmentService = environmentService;
            _solutionInfoService = solutionInfoService;

            TestRunnerTypes = new List<string> { "DotNet", "xUnit", "NUnit" };

            UpdateConfigCommand = new DelegateCommand(UpdateConfig);
            TestProjectChangedCommand = new DelegateCommand<string>(TestProjectChanged);
            AddFileCommand = new DelegateCommand(AddFileToFilter);
            AddLineCommand = new DelegateCommand(AddLineToFilter);
            AddCodeConstrainCommand = new DelegateCommand(AddCodeConstrainToFilter);

            ProjectGridItems = new ObservableCollection<ConfigProjectGridItem>();
            NumberOfParallelTestRuns = 3;
            MutationOperatorGridItems = new ObservableCollection<MutationOperatorGridItem>(Enum
                .GetValues(typeof(MutationOperators)).Cast<MutationOperators>().Select(m =>
                    new MutationOperatorGridItem
                    {
                        IsSelected = true,
                        MutationOperator = m,
                        Description = m.GetValue()
                    }));

            _jsonSettings = new JsonSerializerSettings();

            _jsonSettings.NullValueHandling = NullValueHandling.Ignore;
            _jsonSettings.Converters.Add(new Newtonsoft.Json.Converters.StringEnumConverter());
        }

        public ObservableCollection<ConfigProjectGridItem> ProjectGridItems { get; set; }

        public ObservableCollection<MutationOperatorGridItem> MutationOperatorGridItems { get; set; }

        public DelegateCommand UpdateConfigCommand { get; set; }

        public DelegateCommand AddFileCommand { get; set; }

        public DelegateCommand AddLineCommand { get; set; }

        public DelegateCommand AddCodeConstrainCommand { get; set; }

        public DelegateCommand<string> TestProjectChangedCommand { get; set; }

        public List<string> TestRunnerTypes { get; set; }

        public int SelectedTestRunnerIndex
        {
            get => _selectedTestRunnerIndex;
            set => SetProperty(ref _selectedTestRunnerIndex, value);
        }

        public int SelectedEffectIndex
        {
            get => _selectedEffectIndex;
            set => SetProperty(ref _selectedEffectIndex, value);
        }

        public bool RunBaseline
        {
            get => runBaseline;
            set => SetProperty(ref runBaseline, value);
        }

        public int NumberOfParallelTestRuns
        {
            get => _numberOfParallelTestRuns;
            set => SetProperty(ref _numberOfParallelTestRuns, value);
        }

        public string Filter
        {
            get => _filter;
            set => SetProperty(ref _filter, value);
        }

        public void Initialize()
        {
            _environmentService.JoinableTaskFactory.RunAsync(async () =>
            {
                await _environmentService.JoinableTaskFactory.SwitchToMainThreadAsync();
                _solutionPath = _environmentService.Dte.Solution.FullName;

                var filePath = GetConfigPath();
                _mutationFileConfig = null;

                if (File.Exists(filePath))
                {
                    try
                    {
                        _mutationFileConfig =
                            JsonConvert.DeserializeObject<MutationFileConfig>(File.ReadAllText(filePath));
                    }
                    catch (Exception)
                    {
                        _environmentService.UserNotificationService.ShowWarning("Could not read config file.");
                    }
                }

                var projects = await _solutionInfoService.GetSolutionInfoAsync(_solutionPath);
                _projectNamesInSolution = projects.Select(p => p.Name).ToList();

                ProjectGridItems.Clear();
                foreach (var projectName in _projectNamesInSolution)
                {
                    ProjectGridItems.Add(new ConfigProjectGridItem
                    {
                        IsIgnored = _mutationFileConfig?.IgnoredProjects.Any(u => u == projectName) ?? false,
                        IsTestProject = _mutationFileConfig?.TestProjects.Any(u => u == projectName) ?? projectName.Contains(".Test"),
                        Name = projectName
                    });
                }

                CreateMappings(_mutationFileConfig);

                if (_mutationFileConfig?.Mutators != null)
                {
                    var indexOfTestRunnerTypes = TestRunnerTypes.Select(t => t.ToLower()).ToList().IndexOf(_mutationFileConfig.TestRunner.ToLower());
                    SelectedTestRunnerIndex = indexOfTestRunnerTypes != -1 ? indexOfTestRunnerTypes : 0;
                    NumberOfParallelTestRuns = _mutationFileConfig.NumberOfTestRunInstances > 0 ? _mutationFileConfig.NumberOfTestRunInstances : NumberOfParallelTestRuns;
                    foreach (var mutator in MutationOperatorGridItems)
                    {
                        mutator.IsSelected = _mutationFileConfig.Mutators.FirstOrDefault(m => m == mutator.MutationOperator.ToString()) != null;
                    }
                }

                RunBaseline = _mutationFileConfig?.CreateBaseline ?? true;
                UpdateFilterJson(_mutationFileConfig?.Filter);
            });
        }

        private void CreateMappings(MutationFileConfig mutationFileConfig)
        {
            foreach (var projectGridItem in ProjectGridItems)
            {
                projectGridItem.ProjectMapping = new ObservableCollection<ConfigProjectMappingGridItem>(
                    ProjectGridItems.Where(p => p.IsTestProject).Select(p => new ConfigProjectMappingGridItem { Name = p.Name }));

                if (mutationFileConfig?.ProjectMappings != null &&
                    mutationFileConfig.ProjectMappings.Any(p => p.ProjectName == projectGridItem.Name))
                {
                    var projectsMappingInConfig = mutationFileConfig.ProjectMappings.First(p => p.ProjectName == projectGridItem.Name);

                    foreach (var configProjectMappingGridItem in projectGridItem.ProjectMapping)
                    {
                        configProjectMappingGridItem.IsSelected = projectsMappingInConfig.TestProjectNames.Contains(configProjectMappingGridItem.Name);
                    }
                }
            }
        }

        private void UpdateConfig()
        {
            MutationDocumentFilter filter;

            try
            {
                filter = JsonConvert.DeserializeObject<MutationDocumentFilter>(Filter);
            }
            catch (Exception)
            {
                _environmentService.UserNotificationService.ShowWarning("Can't save the config because of error in filter. Please make sure the filter is correct and then try to update again.");
                return;
            }

            var settings = MutationOperatorGridItems.Where(m => m.IsSelected).Select(m => m.MutationOperator.ToString());

            var config = new MutationFileConfig
            {
                SolutionPath = null,
                IgnoredProjects = ProjectGridItems.Where(s => s.IsIgnored).Select(s => s.Name).ToList(),
                TestProjects = ProjectGridItems.Where(s => s.IsTestProject).Select(s => s.Name).ToList(),
                ProjectMappings = ProjectGridItems
                    .Where(p => p.ProjectMapping.Any(pm => pm.IsSelected))
                    .Select(p => new ProjectMapping
                    {
                        ProjectName = p.Name,
                        TestProjectNames = p.ProjectMapping.Where(pm => pm.IsSelected).Select(pm => pm.Name).ToList()
                    }).ToList(),
                TestRunner = TestRunnerTypes[SelectedTestRunnerIndex],
                CreateBaseline = RunBaseline,
                Mutators = settings.ToList(),
                Filter = filter,
                BuildConfiguration = null,
                NumberOfTestRunInstances = NumberOfParallelTestRuns
            };

            File.WriteAllText(GetConfigPath(), JsonConvert.SerializeObject(config, _jsonSettings));

            _environmentService.UserNotificationService.ShowInfoBar<MutationConfigWindow>("Config updated. Note that updates won't affect any currently open mutation windows.");
        }

        private string GetConfigPath()
        {
            return Path.Combine(Path.GetDirectoryName(_solutionPath), TesturaMutationVsExtensionPackage.BaseConfigName);
        }

        private void TestProjectChanged(string name)
        {
            var project = ProjectGridItems.FirstOrDefault(p => p.Name == name);

            if (project.IsTestProject)
            {
                ProjectGridItems.ForEach(p => p.ProjectMapping.Add(new ConfigProjectMappingGridItem { Name = name }));
                return;
            }

            foreach (var configProjectGridItem in ProjectGridItems)
            {
                var item = configProjectGridItem.ProjectMapping.FirstOrDefault(m => m.Name == name);

                if (item != null)
                {
                    configProjectGridItem.ProjectMapping.Remove(item);
                }
            }
        }

        private void AddCodeConstrainToFilter()
        {
            UpdateFilter(new MutationDocumentFilterItem { CodeConstrain = "CodeToIgnore", Effect = (MutationDocumentFilterItem.FilterEffect)SelectedEffectIndex, Lines = null, Resource = "*" });
        }

        private void AddLineToFilter()
        {
            UpdateFilter(new MutationDocumentFilterItem { CodeConstrain = null, Effect = (MutationDocumentFilterItem.FilterEffect)SelectedEffectIndex, Lines = new List<string> { "59,10", "100", }, Resource = "MyFile.cs" });
        }

        private void AddFileToFilter()
        {
            UpdateFilter(new MutationDocumentFilterItem { CodeConstrain = null, Effect = (MutationDocumentFilterItem.FilterEffect)SelectedEffectIndex, Lines = null, Resource = "MyFile.cs" });
        }

        private void UpdateFilter(MutationDocumentFilterItem item)
        {
            try
            {
                var filter = JsonConvert.DeserializeObject<MutationDocumentFilter>(Filter);

                filter.FilterItems.Add(item);
                UpdateFilterJson(filter);
            }
            catch (Exception)
            {
                _environmentService.UserNotificationService.ShowWarning("Could not add new line because of error in filter.");
            }
        }

        private void UpdateFilterJson(MutationDocumentFilter filter)
        {
            if (filter?.FilterItems == null)
            {
                filter = new MutationDocumentFilter();
            }

            Filter = JsonConvert.SerializeObject(filter, Formatting.Indented, _jsonSettings);
        }
    }
}
