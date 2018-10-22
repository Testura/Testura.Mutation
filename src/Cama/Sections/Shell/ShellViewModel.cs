using System;
using System.Collections.Generic;
using Cama.Helpers.Openers;
using Prism.Mvvm;

namespace Cama.Sections.Shell
{
    public class ShellViewModel : BindableBase
    {
        private readonly CamaProjectOpener _projectOpener;
        private readonly MutationReportOpener _mutationReportOpener;

        public ShellViewModel(
            CamaProjectOpener projectOpener,
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
                if (path.EndsWith(".cama", StringComparison.InvariantCultureIgnoreCase))
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
