using System.IO;
using Newtonsoft.Json;
using Testura.Mutation.Application.Models;
using Testura.Mutation.VsExtension.Sections.Config;
using Testura.Mutation.VsExtension.Wrappers;

namespace Testura.Mutation.VsExtension.Services
{
    public class ConfigService
    {
        private readonly EnvironmentWrapper _environmentWrapper;

        public ConfigService(EnvironmentWrapper environmentWrapper)
        {
            _environmentWrapper = environmentWrapper;
        }

        public bool ConfigExist()
        {
            return _environmentWrapper.JoinableTaskFactory.Run(async () =>
            {
                await _environmentWrapper.JoinableTaskFactory.SwitchToMainThreadAsync();

                if (!File.Exists(Path.Combine(Path.GetDirectoryName(_environmentWrapper.Dte.Solution.FullName), TesturaMutationVsExtensionPackage.BaseConfigName)))
                {
                    _environmentWrapper.UserNotificationService.ShowWarning(
                        "Could not find base config. Please configure Testura.Mutation before running any mutation(s).");

                    await _environmentWrapper.JoinableTaskFactory.SwitchToMainThreadAsync();

                    _environmentWrapper.Dte.ActiveWindow.Close();
                    _environmentWrapper.OpenWindow<MutationConfigWindow>();

                    return false;
                }

                return true;
            });
        }

        public MutationFileConfig GetBaseFileConfig()
        {
            return _environmentWrapper.JoinableTaskFactory.Run(async () =>
                {
                    await _environmentWrapper.JoinableTaskFactory.SwitchToMainThreadAsync();

                    var solutionPath = _environmentWrapper.Dte.Solution.FullName;

                    return JsonConvert.DeserializeObject<MutationFileConfig>(
                        File.ReadAllText(Path.Combine(Path.GetDirectoryName(solutionPath), TesturaMutationVsExtensionPackage.BaseConfigName)));
                });
        }
    }
}
