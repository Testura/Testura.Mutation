using System;
using System.Collections.Generic;
using Prism.Mvvm;
using Unima.Helpers.Openers;

namespace Unima.Sections.Shell
{
    public class ShellViewModel : BindableBase
    {
        private readonly UnimaProjectOpener _projectOpener;
        private readonly MutationReportOpener _mutationReportOpener;

        public ShellViewModel(
            UnimaProjectOpener projectOpener,
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
                if (path.EndsWith(".unima", StringComparison.InvariantCultureIgnoreCase))
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
