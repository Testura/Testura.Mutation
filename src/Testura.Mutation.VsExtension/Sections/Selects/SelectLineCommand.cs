using System;
using System.ComponentModel.Design;
using EnvDTE;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.TextManager.Interop;
using Testura.Mutation.VsExtension.Sections.MutationExplorer;
using Testura.Mutation.VsExtension.Services;
using Task = System.Threading.Tasks.Task;

namespace Testura.Mutation.VsExtension.Sections.Selects
{
    internal sealed class SelectLineCommand
    {
        public const int CommandId = 256;
        public static readonly Guid CommandSet = new Guid("ecfde8a8-d072-4335-b4fb-f268abaecb97");

        private readonly AsyncPackage _package;
        private readonly MutationFilterItemCreatorService _mutationFilterItemCreatorService;

        private SelectLineCommand(
            AsyncPackage package,
            OleMenuCommandService commandService,
            MutationFilterItemCreatorService mutationFilterItemCreatorService)
        {
            _package = package ?? throw new ArgumentNullException(nameof(package));
            _mutationFilterItemCreatorService = mutationFilterItemCreatorService;
            commandService = commandService ?? throw new ArgumentNullException(nameof(commandService));

            var menuCommandID = new CommandID(CommandSet, CommandId);
            var menuItem = new MenuCommand(Execute, menuCommandID);
            commandService.AddCommand(menuItem);
        }

        public static SelectLineCommand Instance { get; private set; }

        private Microsoft.VisualStudio.Shell.IAsyncServiceProvider ServiceProvider => _package;

        public static async Task InitializeAsync(AsyncPackage package, MutationFilterItemCreatorService mutationFilterItemCreatorService)
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync(package.DisposalToken);

            var commandService = await package.GetServiceAsync(typeof(IMenuCommandService)) as OleMenuCommandService;
            Instance = new SelectLineCommand(package, commandService, mutationFilterItemCreatorService);
        }

        private void Execute(object sender, EventArgs e)
        {
            _package.JoinableTaskFactory.Run(async () =>
            {
                await _package.JoinableTaskFactory.SwitchToMainThreadAsync();

                var service = await ServiceProvider.GetServiceAsync(typeof(SVsTextManager));
                var textManager = service as IVsTextManager2;
                textManager.GetActiveView2(1, null, (uint)_VIEWFRAMETYPE.vftCodeWindow, out var view);

                view.GetSelection(out int startLine, out var startColumn, out var endLine, out var endColumn);

                var dte = await ServiceProvider.GetServiceAsync(typeof(DTE)) as DTE;

                if (dte == null)
                {
                    return;
                }

                var file = dte.ActiveDocument.FullName;

                var window =
                    await _package.FindToolWindowAsync(typeof(MutationExplorerWindow), 0, true, _package.DisposalToken) as MutationExplorerWindow;
                if (window?.Frame == null)
                {
                    throw new NotSupportedException("Cannot create tool window");
                }

                var windowFrame = (IVsWindowFrame)window.Frame;
                Microsoft.VisualStudio.ErrorHandler.ThrowOnFailure(windowFrame.Show());

                window.InitializeWindow(_mutationFilterItemCreatorService.CreateFilterFromLines(file, startLine + 1, endLine + 1));
            });
        }
    }
}
