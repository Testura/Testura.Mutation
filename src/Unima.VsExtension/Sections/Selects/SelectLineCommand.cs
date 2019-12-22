using System;
using System.ComponentModel.Design;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.TextManager.Interop;
using Task = System.Threading.Tasks.Task;

namespace Unima.VsExtension.Sections.Selects
{
    internal sealed class SelectLineCommand
    {
        public const int CommandId = 256;
        public static readonly Guid CommandSet = new Guid("ecfde8a8-d072-4335-b4fb-f268abaecb97");
        private readonly AsyncPackage _package;

        private SelectLineCommand(AsyncPackage package, OleMenuCommandService commandService)
        {
            _package = package ?? throw new ArgumentNullException(nameof(package));
            commandService = commandService ?? throw new ArgumentNullException(nameof(commandService));

            var menuCommandID = new CommandID(CommandSet, CommandId);
            var menuItem = new MenuCommand(Execute, menuCommandID);
            commandService.AddCommand(menuItem);
        }

        public static SelectLineCommand Instance { get; private set; }

        private Microsoft.VisualStudio.Shell.IAsyncServiceProvider ServiceProvider => _package;

        public static async Task InitializeAsync(AsyncPackage package)
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync(package.DisposalToken);

            var commandService = await package.GetServiceAsync(typeof(IMenuCommandService)) as OleMenuCommandService;
            Instance = new SelectLineCommand(package, commandService);
        }

        private void Execute(object sender, EventArgs e)
        {
            _package.JoinableTaskFactory.Run(async () =>
            {
                await _package.JoinableTaskFactory.SwitchToMainThreadAsync();

                var service = await ServiceProvider.GetServiceAsync(typeof(SVsTextManager));
                var textManager = service as IVsTextManager2;
                var result = textManager.GetActiveView2(1, null, (uint)_VIEWFRAMETYPE.vftCodeWindow, out var view);

                view.GetSelection(out int startLine, out var startColumn, out var endLine, out var endColumn);
            });
        }
    }
}
