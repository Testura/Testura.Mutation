using System.Windows;
using Dragablz;

namespace Testura.Mutation.Wpf.Sections.Shell
{
    public class MyInterTabClient : IInterTabClient
    {
        public INewTabHost<Window> GetNewHost(IInterTabClient interTabClient, object partition, TabablzControl source)
        {
            var view = new TabShellWindow();
            view.TabsContainer.InterTabController = new InterTabController { InterTabClient = interTabClient };
            return new NewTabHost<Window>(view, view.TabsContainer);
        }

        public TabEmptiedResponse TabEmptiedHandler(TabablzControl tabControl, Window window)
        {
            return TabEmptiedResponse.CloseWindowOrLayoutBranch;
        }
    }
}
