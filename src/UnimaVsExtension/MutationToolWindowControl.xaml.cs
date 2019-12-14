using EnvDTE;
using MediatR;

namespace UnimaVsExtension
{
    using System.Diagnostics.CodeAnalysis;
    using System.Windows;
    using System.Windows.Controls;

    /// <summary>
    /// Interaction logic for MutationToolWindowControl.
    /// </summary>
    public partial class MutationToolWindowControl : UserControl
    {
        private readonly MutationToolWindowControlViewModel _mutationToolWindowControlViewModel;

        public MutationToolWindowControl(MutationToolWindowControlViewModel mutationToolWindowControlViewModel)
        {
            _mutationToolWindowControlViewModel = mutationToolWindowControlViewModel;

            this.DataContext = _mutationToolWindowControlViewModel;
            this.InitializeComponent();
        }

        public void Initialize(string solutionPath)
        {
            _mutationToolWindowControlViewModel.Initialize(solutionPath);
        }
    }
}