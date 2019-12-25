using System.IO;
using Newtonsoft.Json;
using Unima.Application.Models;
using Unima.VsExtension.Sections.Config;
using Unima.VsExtension.Wrappers;

namespace Unima.VsExtension.Services
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

                if (!File.Exists(Path.Combine(Path.GetDirectoryName(_environmentWrapper.Dte.Solution.FullName), UnimaVsExtensionPackage.BaseConfigName)))
                {
                    _environmentWrapper.UserNotificationService.ShowWarning(
                        "Could not find base config. Please configure unima before running any mutation(s).");

                    await _environmentWrapper.JoinableTaskFactory.SwitchToMainThreadAsync();

                    _environmentWrapper.Dte.ActiveWindow.Close();
                    _environmentWrapper.OpenWindow<UnimaConfigWindow>();

                    return false;
                }

                return true;
            });
        }

        public UnimaFileConfig GetBaseFileConfig()
        {
            return _environmentWrapper.JoinableTaskFactory.Run(async () =>
                {
                    await _environmentWrapper.JoinableTaskFactory.SwitchToMainThreadAsync();

                    var solutionPath = _environmentWrapper.Dte.Solution.FullName;

                    return JsonConvert.DeserializeObject<UnimaFileConfig>(
                        File.ReadAllText(Path.Combine(Path.GetDirectoryName(solutionPath), UnimaVsExtensionPackage.BaseConfigName)));
                });
        }
    }
}
