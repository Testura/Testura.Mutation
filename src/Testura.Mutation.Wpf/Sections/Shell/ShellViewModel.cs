using System;
using System.Collections.Generic;
using Prism.Mvvm;
using Testura.Mutation.Wpf.Helpers.Openers;

namespace Testura.Mutation.Wpf.Sections.Shell
{
    public class ShellViewModel : BindableBase
    {
        private readonly TesturaMutationProjectOpener _projectOpener;
        private readonly MutationReportOpener _mutationReportOpener;

        public ShellViewModel(
            TesturaMutationProjectOpener projectOpener,
            MutationReportOpener mutationReportOpener)
        {
            _projectOpener = projectOpener;
            _mutationReportOpener = mutationReportOpener;
            MyInterTabClient = new MyInterTabClient();
        }

        public MyInterTabClient MyInterTabClient { get; set; }

        public void Open(IEnumerable<string> paths)
        {
            foreach (var path in paths)
            {
                // Change this to a constant somewhere?
                if (path.EndsWith(".Testura.Mutation", StringComparison.InvariantCultureIgnoreCase))
                {
                    OpenReport(path);
                    continue;
                }

                OpenProject(path);
            }
        }

        public void OpenProject(string path)
        {
           _projectOpener.OpenProject(path);
        }

        public void OpenReport(string path)
        {
            _mutationReportOpener.OpenMutationReport(path);
        }
    }
}
