using System;
using System.IO;
using Newtonsoft.Json;
using Testura.Mutation.Application.Models;
using Testura.Mutation.VsExtension.Sections.Config;

namespace Testura.Mutation.VsExtension.Services
{
    public class ConfigService
    {
        private readonly EnvironmentService _environmentService;

        public ConfigService(EnvironmentService environmentService)
        {
            _environmentService = environmentService;
        }

        public bool ConfigExist()
        {
            return _environmentService.JoinableTaskFactory.Run(async () =>
            {
                await _environmentService.JoinableTaskFactory.SwitchToMainThreadAsync();

                if (!File.Exists(Path.Combine(Path.GetDirectoryName(_environmentService.Dte.Solution.FullName), TesturaMutationVsExtensionPackage.BaseConfigName)))
                {
                    _environmentService.UserNotificationService.ShowWarning(
                        "Could not find base config. Please configure Testura.Mutation before running any mutation(s).");

                    await _environmentService.JoinableTaskFactory.SwitchToMainThreadAsync();

                    _environmentService.Dte.ActiveWindow.Close();
                    var window = _environmentService.OpenWindow<MutationConfigWindow>();
                    window.InitializeWindow();

                    return false;
                }

                return true;
            });
        }

        public MutationFileConfig GetBaseFileConfig()
        {
            var buildConfiguration = _environmentService.GetCurrentSolutionConfiguration();

            return _environmentService.JoinableTaskFactory.Run(async () =>
                {
                    await _environmentService.JoinableTaskFactory.SwitchToMainThreadAsync();

                    var solutionPath = _environmentService.Dte.Solution.FullName;

                    try
                    {
                        var baseConfig = JsonConvert.DeserializeObject<MutationFileConfig>(File.ReadAllText(Path.Combine(Path.GetDirectoryName(solutionPath), TesturaMutationVsExtensionPackage.BaseConfigName)));

                        baseConfig.SolutionPath = solutionPath;
                        baseConfig.BuildConfiguration = buildConfiguration;

                        return baseConfig;
                    }
                    catch (Exception)
                    {
                        _environmentService.UserNotificationService.ShowError("Failed to load config. Make sure it's a valid json");
                        _environmentService.Dte.ActiveWindow.Close();

                        return null;
                    }
                });
        }
    }
}
