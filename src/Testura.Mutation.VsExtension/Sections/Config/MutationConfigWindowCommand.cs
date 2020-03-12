using System;
using System.ComponentModel.Design;
using Microsoft.VisualStudio.Shell;
using Testura.Mutation.Core.Solution;
using Testura.Mutation.VsExtension.Services;
using IAsyncServiceProvider = Microsoft.VisualStudio.Shell.IAsyncServiceProvider;
using Task = System.Threading.Tasks.Task;

namespace Testura.Mutation.VsExtension.Sections.Config
{
    internal sealed class MutationConfigWindowCommand
    {
        public const int CommandId = 4130;
        public static readonly Guid CommandSet = new Guid("eb065996-2187-4c2c-8ecf-947ac6264c49");

        private readonly AsyncPackage _package;
        private readonly UserNotificationService _userNotificationService;
        private readonly EnvironmentService _environmentService;
        private readonly SolutionInfoService _solutionInfoService;

        private MutationConfigWindowCommand(
            AsyncPackage package,
            OleMenuCommandService commandService,
            UserNotificationService userNotificationService,
            EnvironmentService environmentService,
            SolutionInfoService solutionInfoService)
        {
            _package = package ?? throw new ArgumentNullException(nameof(package));
            _userNotificationService = userNotificationService;
            _environmentService = environmentService;
            _solutionInfoService = solutionInfoService;
            commandService = commandService ?? throw new ArgumentNullException(nameof(commandService));

            var menuCommandID = new CommandID(CommandSet, CommandId);
            var menuItem = new MenuCommand(Execute, menuCommandID);
            commandService.AddCommand(menuItem);
        }

        public static MutationConfigWindowCommand Instance { get; private set; }

        private IAsyncServiceProvider ServiceProvider => _package;

        public static async Task InitializeAsync(AsyncPackage package, UserNotificationService userNotificationService, EnvironmentService environmentService, SolutionInfoService solutionInfoService)
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync(package.DisposalToken);

            OleMenuCommandService commandService = await package.GetServiceAsync(typeof(IMenuCommandService)) as OleMenuCommandService;
            Instance = new MutationConfigWindowCommand(package, commandService, userNotificationService, environmentService, solutionInfoService);
        }

        private void Execute(object sender, EventArgs e)
        {
            try
            {
                var mutationConfigControl = new MutationConfigWindowControl(new MutationConfigWindowViewModel(_environmentService, _solutionInfoService));
                mutationConfigControl.ShowDialog();
            }
            catch (Exception ex)
            {
                _userNotificationService.ShowError($"Failed to open config window. Make sure that you have the latest visual studio update. \n Exception: \n {ex.Message}");
                throw;
            }
        }
    }
}
